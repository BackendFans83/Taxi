import { JSX, splitProps, Show } from 'solid-js';
import { Portal } from 'solid-js/web';
import styles from './Dialog.module.css';

export type DialogProps = {
  open: boolean;
  onClose?: () => void;
  title?: string;
  children?: JSX.Element;
  actions?: JSX.Element;
  class?: string;
  overlayClass?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Dialog(props: DialogProps) {
  const [local, others] = splitProps(props, [
    'open', 'onClose', 'title', 'children', 'actions', 'class', 'overlayClass'
  ]);

  const handleOverlayClick = (e: MouseEvent) => {
    if (e.target === e.currentTarget) {
      local.onClose?.();
    }
  };

  return (
    <Show when={local.open}>
      <Portal>
        <div
          class={`${styles['dialog-overlay']} ${local.overlayClass || ''} ${local.open ? styles.active : ''}`}
          onClick={handleOverlayClick}
        >
          <div class={`${styles.dialog} ${local.class || ''}`} {...others}>
            <Show when={local.title}>
              <h3 class={styles['dialog-title']}>{local.title}</h3>
            </Show>
            <div class={styles['dialog-content']}>
              {local.children}
            </div>
            <Show when={local.actions}>
              <div class={styles['dialog-actions']}>
                {local.actions}
              </div>
            </Show>
          </div>
        </div>
      </Portal>
    </Show>
  );
}