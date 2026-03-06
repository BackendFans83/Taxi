import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Progress.module.css';

export type LinearProgressProps = {
  /** Значение прогресса от 0 до 100 (если не задано, то анимация бесконечная) */
  value?: number;
  /** Дополнительный класс */
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function LinearProgress(props: LinearProgressProps) {
  const [local, others] = splitProps(props, ['value', 'class']);
  const classList = createMemo(() => [styles['linear-progress'], local.class].filter(Boolean).join(' '));

  const barClassList = createMemo(() => [
    styles['linear-progress-bar'],
    local.value === undefined ? styles.indeterminate : '',
    local.value === undefined && styles['linear-progress-bar-indeterminate'],
  ].filter(Boolean).join(' '));

  const barStyle = () => {
    if (local.value !== undefined) {
      return { width: `${Math.min(100, Math.max(0, local.value))}%` };
    }
    return {};
  };

  return (
    <div class={classList()} {...others}>
      <div class={barClassList()} style={barStyle()} />
    </div>
  );
}