import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Button.module.css';

export type ButtonVariant = 'primary' | 'contained' | 'outlined' | 'text';

const variantsClasses = {
    text: styles['btn-text'],
    primary: styles['btn-primary'],
    contained: styles['btn-contained'],
    outlined: styles['btn-outlined'],
    disabled: styles['btn-disabled'],
} as const;

export type ButtonProps = {
    variant?: ButtonVariant;
    disabled?: boolean;
    class?: string;
} & JSX.ButtonHTMLAttributes<HTMLButtonElement>;

const BASE_CLASS = styles.btn;

export function Button(props: ButtonProps) {
    const [local, others] = splitProps(props, ['variant', 'class', 'disabled', 'children']);
    const variantKey = createMemo(() => local.disabled ? 'disabled' : (local.variant ?? 'contained'));
    const classList = createMemo(() => [
        BASE_CLASS,
        variantsClasses[variantKey()],
        local.class,
    ].filter(Boolean).join(' '));

    return (
        <button
            class={classList()}
            disabled={local.disabled}
            {...(others as any)}
        >
            {local.children}
        </button>
    );
}

Button.displayName = 'Button';

export default Button;