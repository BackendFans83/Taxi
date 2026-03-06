import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Tabs.module.css';

export type TabProps = {
  active?: boolean;
  disabled?: boolean;
  onClick?: (e: MouseEvent) => void;
  children: JSX.Element;
  class?: string;
} & JSX.ButtonHTMLAttributes<HTMLButtonElement>;

export function Tab(props: TabProps) {
  const [local, others] = splitProps(props, ['active', 'disabled', 'onClick', 'children', 'class']);
  const classList = createMemo(() => 
    [
      styles.tab,
      local.active && styles['tab-active'],
      local.disabled && styles['tab-disabled'],
      local.class,
    ]
      .filter(Boolean)
      .join(' ')
  );
  return (
    <button
      class={classList()}
      onClick={local.onClick}
      disabled={local.disabled}
      {...others}
    > 
      {local.children}
    </button>
  );
}

Tab.displayName = 'Tab';

export default Tab;