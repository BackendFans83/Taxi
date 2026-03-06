import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Divider.module.css';

export type DividerProps = {
  orientation?: 'horizontal' | 'vertical';
  thickness?: string | number;
  color?: string;
  margin?: string | number;
  class?: string;
} & JSX.HTMLAttributes<HTMLHRElement>;

export function Divider(props: DividerProps) {
  const [local, others] = splitProps(props, [
    'orientation', 'thickness', 'color', 'margin', 'class'
  ]);

  const isHorizontal = local.orientation !== 'vertical';

  const thicknessValue = local.thickness !== undefined
    ? (typeof local.thickness === 'number' ? `${local.thickness}px` : local.thickness)
    : '1px';
  const colorValue = local.color ?? 'var(--divider)';
  const marginValue = local.margin !== undefined
    ? (typeof local.margin === 'number' ? `${local.margin}px` : local.margin)
    : (isHorizontal ? '24px 0' : '0 24px');

  const style: JSX.CSSProperties = {
    ...(isHorizontal
      ? { height: thicknessValue, width: '100%', margin: marginValue }
      : { width: thicknessValue, height: '100%', margin: marginValue }
    ),
    background: colorValue,
    border: 'none',
  };

  const classList = createMemo(() => [styles.divider, local.class].filter(Boolean).join(' '));

  return <hr class={classList()} style={style} {...others} />;
}