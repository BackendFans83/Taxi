import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './List.module.css';

export type ListItemProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLLIElement>;

export function ListItem(props: ListItemProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() =>
    [
      styles['list-item'],
      local.class
    ]
      .filter(Boolean)
      .join(' ')
  );
  return (
    <li class={classList()} {...others}>
      {local.children}
    </li>
  );
}