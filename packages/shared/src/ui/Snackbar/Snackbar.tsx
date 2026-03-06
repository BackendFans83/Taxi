import { JSX, splitProps, createEffect, onCleanup, createMemo } from 'solid-js';
import { Portal } from 'solid-js/web';
import styles from './Snackbar.module.css';

export type SnackbarType = 'success' | 'error' | 'warning' | 'info';

export type SnackbarProps = {
    /** Открыт ли снекбар */
    open: boolean;
    /** Текст сообщения */
    message: string;
    /** Тип снекбара */
    type?: SnackbarType;
    /** Длительность показа (мс) */
    duration?: number;
    /** Колбэк при закрытии (после таймаута или при клике) */
    onClose?: () => void;
    /** Иконка (по умолчанию для типа) */
    icon?: JSX.Element;
    /** Возможность закрыть вручную */
    closable?: boolean;
    /** Дополнительный класс */
    class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

const defaultIcons: Record<SnackbarType, JSX.Element> = {
    info: (
        <svg class={styles.icon} viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" />
        </svg>
    ),
    success: (
        <svg class={styles.icon} viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" />
        </svg>
    ),
    warning: (
        <svg class={styles.icon} viewBox="0 0 24 24">
            <path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z" />
        </svg>
    ),
    error: (
        <svg class={styles.icon} viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" />
        </svg>
    ),
};

export function Snackbar(props: SnackbarProps) {
    const [local, others] = splitProps(props, [
        'open', 'message', 'type', 'duration', 'onClose', 'icon', 'closable', 'class'
    ]);

    const type = local.type ?? 'info';
    const icon = local.icon ?? defaultIcons[type];
    let timeoutId: number | undefined;

    createEffect(() => {
        if (local.open && local.duration !== 0) {
            timeoutId = window.setTimeout(() => {
                local.onClose?.();
            }, local.duration ?? 3000);
        }
        onCleanup(() => {
            if (timeoutId !== undefined) {
                clearTimeout(timeoutId);
            }
        });
    });

    const handleClose = () => {
        if (timeoutId) clearTimeout(timeoutId);
        local.onClose?.();
    };

    const containerClassesList = createMemo(() => 
        [
            styles.snackbar,
            styles[`snackbar-${type}`],
            local.open && styles.active,
            local.class,
        ]
        .filter(Boolean)
        .join(' ')
    );

    return (
        <Portal>
            <div class={containerClassesList()} {...others}>
                {icon}
                <span class={styles.message}>{local.message}</span>
                {local.closable && (
                    <button class={styles.closeBtn} onClick={handleClose}>✕</button>
                )}
            </div>
        </Portal>
    );
}
