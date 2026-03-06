import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './List.module.css';

export type ListProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLUListElement>;

export function List(props: ListProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [styles.list, local.class].filter(Boolean).join(' '));
  return (
    <ul class={classList()} {...others}>
      {local.children}
    </ul>
  );
}