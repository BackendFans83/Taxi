import axios, { AxiosRequestConfig, Method } from 'axios';

const BASE_URL = '/api/v1';

interface HttpOptions extends Omit<AxiosRequestConfig, 'method' | 'url' | 'params'> {
  params?: Record<string, string>;
}

interface CacheEntry<T> {
  data: T;
  timestamp: number;
}

class HttpClient {
  private baseURL: string;
  private cache = new Map<string, CacheEntry<unknown>>();
  private defaultTTL = 5 * 60 * 1000; // 5 минут по умолчанию
  
  // Эндпоинты, которые не кэшируем (мутации)
  private noCacheMethods = new Set(['POST', 'PUT', 'DELETE', 'PATCH']);
  
  // Эндпоинты с индивидуальным TTL (мс)
  private customTTL: Record<string, number> = {
    '/auth/me': 2 * 60 * 1000,      // 2 минуты для данных пользователя
    '/users': 3 * 60 * 1000,        // 3 минуты для пользователей
  };

  constructor(baseURL: string = BASE_URL) {
    this.baseURL = baseURL;
  }

  private getCacheKey(endpoint: string, params?: Record<string, string>): string {
    return `${endpoint}:${JSON.stringify(params || {})}`;
  }

  private getTTL(endpoint: string): number {
    // Ищем подходящее правило по префиксу
    for (const [pattern, ttl] of Object.entries(this.customTTL)) {
      if (endpoint.startsWith(pattern)) {
        return ttl;
      }
    }
    return this.defaultTTL;
  }

  private getFromCache<T>(key: string, ttl: number): T | null {
    const entry = this.cache.get(key) as CacheEntry<T> | undefined;
    if (!entry) return null;
    
    const age = Date.now() - entry.timestamp;
    if (age > ttl) {
      this.cache.delete(key);
      return null;
    }
    
    return entry.data;
  }

  private setCache<T>(key: string, data: T): void {
    this.cache.set(key, { data, timestamp: Date.now() });
  }

  /**
   * Очистка кэша
   * @param pattern - шаблон эндпоинта (например, '/users' очистит все /users/*)
   * Если не указан - очищает весь кэш
   */
  clearCache(pattern?: string): void {
    if (!pattern) {
      this.cache.clear();
      return;
    }
    
    for (const key of this.cache.keys()) {
      if (key.startsWith(pattern)) {
        this.cache.delete(key);
      }
    }
  }

  /**
   * Инвалидация кэша после мутаций
   * Автоматически вызывается при POST/PUT/DELETE/PATCH
   */
  private invalidateCache(endpoint: string, method: Method): void {
    if (this.noCacheMethods.has(method)) {
      // Извлекаем базовый путь (например, /users/123 -> /users)
      const basePath = endpoint.replace(/\/\d+$/, '');
      this.clearCache(basePath);
      
      // Также инвалидируем связанные эндпоинты
      if (basePath.includes('/auth')) {
        this.clearCache('/auth/me');
      }
    }
  }

  private async request<T>(endpoint: string, method: Method, options: HttpOptions = {}): Promise<T> {
    const { params, ...axiosConfig } = options;
    const cacheKey = this.getCacheKey(endpoint, params);

    // Для GET запросов проверяем кэш
    if (method === 'GET') {
      const ttl = this.getTTL(endpoint);
      const cachedData = this.getFromCache<T>(cacheKey, ttl);
      
      if (cachedData !== null) {
        console.log(`[HTTP Cache] HIT: ${endpoint}`);
        return cachedData;
      }
      
      console.log(`[HTTP Cache] MISS: ${endpoint}`);
    }

    try {
      const response = await axios.request<T>({
        url: endpoint,
        method,
        baseURL: this.baseURL,
        params,
        ...axiosConfig,
      });

      // Кэшируем успешные GET запросы
      if (method === 'GET' && response.data !== undefined) {
        this.setCache(cacheKey, response.data);
      }

      // Инвалидируем кэш при мутациях
      this.invalidateCache(endpoint, method);

      return response.data;
    } catch (error) {
      // При ошибке пробуем вернуть устаревшие данные из кэша (stale-while-revalidate)
      if (method === 'GET') {
        const staleData = this.cache.get(cacheKey) as CacheEntry<T> | undefined;
        if (staleData) {
          console.warn(`[HTTP Cache] STALE: ${endpoint} (используем устаревшие данные)`);
          return staleData.data;
        }
      }
      throw error;
    }
  }

  get<T>(endpoint: string, options: HttpOptions = {}): Promise<T> {
    return this.request<T>(endpoint, 'GET', options);
  }

  post<T>(endpoint: string, data?: unknown, options: HttpOptions = {}): Promise<T> {
    return this.request<T>(endpoint, 'POST', { ...options, data });
  }

  put<T>(endpoint: string, data?: unknown, options: HttpOptions = {}): Promise<T> {
    return this.request<T>(endpoint, 'PUT', { ...options, data });
  }

  delete<T>(endpoint: string, options: HttpOptions = {}): Promise<T> {
    return this.request<T>(endpoint, 'DELETE', options);
  }
  
  /**
   * Патч запрос с поддержкой кэширования
   */
  patch<T>(endpoint: string, data?: unknown, options: HttpOptions = {}): Promise<T> {
    return this.request<T>(endpoint, 'PATCH', { ...options, data });
  }
}

export const http = new HttpClient();
export { HttpClient };
export default http;