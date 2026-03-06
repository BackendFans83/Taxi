import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Tabs.module.css';

export type TabsProps = {
  children: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Tabs(props: TabsProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => 
    [
       styles.tabs,
       local.class,
    ].filter(Boolean).join(' ')
  );
  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}

export default Tabs;