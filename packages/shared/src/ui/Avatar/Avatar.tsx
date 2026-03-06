import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Avatar.module.css';

export type AvatarSize = 'sm' | 'md' | 'lg';

export type AvatarProps = {
  /** URL изображения */
  src?: string;
  /** Альтернативный текст */
  alt?: string;
  /** Инициалы (если нет изображения) */
  initials?: string;
  /** Размер аватара */
  size?: AvatarSize;
  /** Дополнительные классы */
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

const sizeClasses = {
  sm: styles['avatar-sm'],
  md: styles['avatar-md'],
  lg: styles['avatar-lg'],
};

export function Avatar(props: AvatarProps) {
  const [local, others] = splitProps(props, ['src', 'alt', 'initials', 'size', 'class', 'children']);

  const size = local.size ?? 'md';
  const containerClass = createMemo(() => [
    styles.avatar,
    sizeClasses[size],
    local.class,
  ].filter(Boolean).join(' '));

  return (
    <div class={containerClass()} {...others}>
      {local.src ? (
        <img src={local.src} alt={local.alt ?? 'avatar'} />
      ) : (
        <span>{local.initials || local.children}</span>
      )}
    </div>
  );
}