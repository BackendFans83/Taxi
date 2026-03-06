import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Divider.module.css';

export type DividerWithTextProps = {
  /** Текст или элемент внутри разделителя */
  children: JSX.Element;
  /** Цвет линий */
  color?: string;
  /** Цвет текста */
  textColor?: string;
  /** Размер текста */
  textSize?: string;
  /** Расстояние между текстом и линиями */
  gap?: string | number;
  /** Дополнительный класс */
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function DividerWithText(props: DividerWithTextProps) {
  const [local, others] = splitProps(props, [
    'children', 'color', 'textColor', 'textSize', 'gap', 'class'
  ]);

  const lineColor = local.color ?? 'var(--divider)';
  const textColorValue = local.textColor ?? 'var(--text-secondary)';
  const textSizeValue = local.textSize ?? '12px';
  const gapValue = typeof local.gap === 'number' ? `${local.gap}px` : (local.gap ?? '16px');

  const containerStyle = {
    display: 'flex',
    'align-items': 'center',
    gap: gapValue,
    width: '100%',
    margin: '24px 0',
  };

  const lineStyle = {
    flex: 1,
    height: '1px',
    background: lineColor,
  };

  const textStyle = {
    color: textColorValue,
    'font-size': textSizeValue,
    'white-space': 'nowrap' as const,
  };

  const classList = createMemo(() => [styles['divider-with-text'], local.class].filter(Boolean).join(' '));

  return (
    <div class={classList()} style={containerStyle} {...others}>
      <span style={lineStyle} />
      <span style={textStyle}>{local.children}</span>
      <span style={lineStyle} />
    </div>
  );
}