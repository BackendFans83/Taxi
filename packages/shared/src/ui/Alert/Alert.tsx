import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Alert.module.css';

export type AlertType = 'info' | 'success' | 'warning' | 'error';

export type AlertProps = {
  /** Тип алерта */
  type?: AlertType;
  /** Сообщение (либо дети) */
  children?: JSX.Element;
  /** Иконка (по умолчанию подставляется для каждого типа) */
  icon?: JSX.Element;
  /** Дополнительный класс */
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

// Иконки по умолчанию (можно вынести в отдельный файл)
const defaultIcons: Record<AlertType, JSX.Element> = {
  info: (
    <svg class={styles.icon} viewBox="0 0 24 24">
      <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" />
    </svg>
  ),
  success: (
    <svg class={styles.icon} viewBox="0 0 24 24">
      <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" />
    </svg>
  ),
  warning: (
    <svg class={styles.icon} viewBox="0 0 24 24">
      <path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z" />
    </svg>
  ),
  error: (
    <svg class={styles.icon} viewBox="0 0 24 24">
      <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" />
    </svg>
  ),
};

export function Alert(props: AlertProps) {
  const [local, others] = splitProps(props, ['type', 'children', 'icon', 'class']);
  const type = local.type ?? 'info';
  const icon = local.icon ?? defaultIcons[type];

  const classList = createMemo(() => [
    styles.alert,
    styles[`alert-${type}`],
    local.class,
  ].filter(Boolean).join(' '));

  return (
    <div class={classList()} {...others}>
      {icon}
      <span class={styles['alert-message']}>{local.children}</span>
    </div>
  );
}