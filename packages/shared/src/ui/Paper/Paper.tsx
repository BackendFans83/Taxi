import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Paper.module.css';

export type PaperProps = {
  children?: JSX.Element;
  class?: string;
  /** Уровень тени: 'none' | 'sm' | 'md' | 'lg' (по умолчанию 'md') */
  elevation?: 'none' | 'sm' | 'md' | 'lg';
  /** Отступы: 'none' | 'sm' | 'md' | 'lg' (по умолчанию 'md') */
  padding?: 'none' | 'sm' | 'md' | 'lg';
  /** Флаг: есть ли граница (по умолчанию false) */
  outlined?: boolean;
} & JSX.HTMLAttributes<HTMLDivElement>;

const elevationClasses = {
  none: styles['paper-elevation-none'],
  sm: styles['paper-elevation-sm'],
  md: styles['paper-elevation-md'],
  lg: styles['paper-elevation-lg'],
};

const paddingClasses = {
  none: styles['paper-padding-none'],
  sm: styles['paper-padding-sm'],
  md: styles['paper-padding-md'],
  lg: styles['paper-padding-lg'],
};

export function Paper(props: PaperProps) {
  const [local, others] = splitProps(props, [
    'children', 'class', 'elevation', 'padding', 'outlined'
  ]);

  const classList = createMemo(() => [
    styles.paper,
    local.outlined && styles['paper-outlined'],
    elevationClasses[local.elevation ?? 'md'],
    paddingClasses[local.padding ?? 'md'],
    local.class,
  ].filter(Boolean).join(' '));

  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}