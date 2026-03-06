import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Button.module.css';

export type IconButtonVariant = 'primary' | 'secondary' | 'default';

export type IconButtonProps = {
    disabled?: boolean;
    variant?: IconButtonVariant;
    icon?: JSX.Element;
    class?: string;
} & JSX.ButtonHTMLAttributes<HTMLButtonElement>;

const variantsClasses = {
    primary: styles['icon-btn-primary'],
    secondary: styles['icon-btn-secondary'],
    disabled: styles['icon-btn-disabled'],
} as const;

const BASE_CLASS = styles['icon-btn'];

export function IconButton(props: IconButtonProps) {
    const [local, others] = splitProps(props, ['variant', 'class', 'disabled', 'icon', 'children']);

    const classList = createMemo(() => [
        BASE_CLASS,
        local.disabled && variantsClasses.disabled,
        local.variant && local.variant !== 'default' && variantsClasses[local.variant],
        local.class,
    ].filter(Boolean).join(' '));

    return (
        <button
            class={classList()}
            disabled={local.disabled}
            {...(others as any)}
        >
            {local.icon}
            {local.children}
        </button>
    );
}

IconButton.displayName = 'IconButton';

export default IconButton;