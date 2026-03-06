import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Stack.module.css';

export type StackProps = {
  direction?: 'row' | 'column';
  gap?: string | number;
  wrap?: boolean;
  alignItems?: 'flex-start' | 'flex-end' | 'center' | 'stretch' | 'baseline';
  justifyContent?: 'flex-start' | 'flex-end' | 'center' | 'space-between' | 'space-around' | 'space-evenly';
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Stack(props: StackProps) {
  const [local, others] = splitProps(props, [
    'direction', 'gap', 'wrap', 'alignItems', 'justifyContent', 'children', 'class'
  ]);

  const gapValue = typeof local.gap === 'number' ? `${local.gap}px` : (local.gap ?? '16px');
  const wrapValue = local.wrap ? ('wrap' as const) : ('nowrap' as const);
  const directionValue = local.direction ?? ('column' as const);

  const style = {
    display: 'flex',
    'flex-direction': directionValue,
    gap: gapValue,
    'flex-wrap': wrapValue,
    'align-items': local.alignItems,
    'justify-content': local.justifyContent,
  };

  const classList = createMemo(() => [styles.stack, local.class].filter(Boolean).join(' '));

  return (
    <div class={classList()} style={style} {...others}>
      {local.children}
    </div>
  );
}