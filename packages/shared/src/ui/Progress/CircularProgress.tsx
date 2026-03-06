import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Progress.module.css';

export type CircularProgressSize = 'sm' | 'md' | 'lg';

export type CircularProgressProps = {
  /** Размер индикатора */
  size?: CircularProgressSize;
  /** Дополнительный класс */
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

const sizeClasses = {
  sm: styles['circular-progress-sm'],
  md: styles['circular-progress-md'],
  lg: styles['circular-progress-lg'],
};

export function CircularProgress(props: CircularProgressProps) {
  const [local, others] = splitProps(props, ['size', 'class']);
  const size = local.size ?? 'md';
  
  const classList = createMemo(() => [
    styles['circular-progress'],
    sizeClasses[size],
    local.class,
  ].filter(Boolean).join(' '));

  return <div class={classList()} {...others} />;
}