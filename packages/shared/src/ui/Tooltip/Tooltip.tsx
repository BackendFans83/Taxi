import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Tooltip.module.css';

export type TooltipProps = {
  /** Текст подсказки */
  title: string;
  /** Дочерний элемент, к которому привязан tooltip */
  children: JSX.Element;
  /** Позиция подсказки */
  placement?: 'top' | 'bottom' | 'left' | 'right';
  /** Дополнительный класс */
  class?: string;
} & JSX.HTMLAttributes<HTMLSpanElement>;

const placementClasses = {
  top: styles['tooltip-top'],
  bottom: styles['tooltip-bottom'],
  left: styles['tooltip-left'],
  right: styles['tooltip-right'],
};

export function Tooltip(props: TooltipProps) {
  const [local, others] = splitProps(props, ['title', 'children', 'placement', 'class']);

  const containerClass = createMemo(() => [styles.tooltip, local.class].filter(Boolean).join(' '));

  const textClass = createMemo(() => {
    return [
      styles['tooltip-text'],
      placementClasses[local.placement ?? 'top'],
    ].filter(Boolean).join(' ');
  });


  return (
    <span class={containerClass()} {...others}>
      {local.children}
      <span class={textClass()}>{local.title}</span>
    </span>
  );
}