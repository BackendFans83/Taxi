import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Badge.module.css';

export type BadgeProps = {
    /** Содержимое бейджа (обычно число или короткий текст) */
    badgeContent?: string | number;
    /** Дочерний элемент, к которому применяется бейдж */
    children?: JSX.Element;
    /** Максимальное значение для отображения (например, +99) */
    max?: number;
    /** Дополнительный класс для контейнера */
    class?: string;
    /** Показывать ли бейдж, если контент пустой или 0 */
    showZero?: boolean;
    /** Цвет бейджа (по умолчанию error) */
    color?: 'error' | 'primary' | 'secondary' | 'success' | 'warning';
} & JSX.HTMLAttributes<HTMLSpanElement>;

const colorClasses = {
    error: styles['badge-error'],
    primary: styles['badge-primary'],
    secondary: styles['badge-secondary'],
    success: styles['badge-success'],
    warning: styles['badge-warning'],
};

export function Badge(props: BadgeProps) {
    const [local, others] = splitProps(props, [
        'badgeContent', 'children', 'max', 'class', 'showZero', 'color'
    ]);

    const shouldShow = () => {
        if (local.badgeContent === undefined) return false;
        if (local.badgeContent === '' || local.badgeContent === 0) return local.showZero;
        return true;
    };

    const displayContent = () => {
        if (local.max !== undefined && typeof local.badgeContent === 'number' && local.badgeContent > local.max) {
            return `${local.max}+`;
        }
        return local.badgeContent;
    };

    const containerClass = createMemo(() => [styles.badge, local.class].filter(Boolean).join(' '));
    const dotClass = createMemo(() => [
        styles['badge-dot'],
        colorClasses[local.color ?? 'error'],
    ].filter(Boolean).join(' '));

    return (
        <span class={containerClass()} {...others}>
            {local.children}
            {shouldShow() && (
                <span class={dotClass()}>
                    {displayContent()}
                </span>
            )}
        </span>
    );
}