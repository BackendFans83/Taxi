import { createEffect, createSignal, JSX,createMemo, splitProps } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './TextField.module.css';

export type TextFieldType = 'text' | 'password' | 'email' | 'number' | 'tel' | 'url' | 'search';

export type TextFieldProps = {
    type?: TextFieldType;
    value?: string;               // controlled
    defaultValue?: string;        // uncontrolled initial value
    name?: string;
    multiline?: boolean;
    label?: string;
    placeholder?: string;
    error?: string;               // external error (overrides internal)
    class?: string;
    disabled?: boolean;
    required?: boolean;
    pattern?: string;              // regex pattern as string
    validate?: (value: string) => string | null; // custom validator, returns error message or null
    onInput?: (e: Event) => void;
    onBlur?: (e: Event) => void;
    onError?: (error: string | null) => void; // callback when internal error changes
} & JSX.InputHTMLAttributes<HTMLInputElement>;

const BASE_CLASS = styles['text-field'];
const variantClasses = {
    disabled: styles['text-field-disabled'],
    error: styles['text-field-error'],
};

export function TextField(props: TextFieldProps) {
    const [local, others] = splitProps(props, [
        'class', 'disabled', 'placeholder', 'error', 'label',
        'value', 'defaultValue', 'name', 'type', 'multiline',
        'required', 'pattern', 'validate', 'onInput', 'onBlur', 'onError'
    ]);

    const [internalValue, setInternalValue] = createSignal(local.defaultValue ?? '');
    const [internalError, setInternalError] = createSignal<string | null>(null);
    const [touched, setTouched] = createSignal(false);

    const effectiveValue = () => local.value !== undefined ? local.value : internalValue();
    const effectiveError = () => local.error !== undefined ? local.error : internalError();

    const validateValue = (val: string): string | null => {
        if (local.required && !val) return 'This field is required';
        if (local.pattern && !new RegExp(local.pattern).test(val)) {
            return `Invalid format (pattern: ${local.pattern})`;
        }
        if (local.validate) return local.validate(val);
        return null;
    };

    const updateValidation = (val: string) => {
        const err = validateValue(val);
        setInternalError(err);
        local.onError?.(err);
    };

    createEffect(() => {
        if (local.value !== undefined) {
            updateValidation(local.value);
        }
    });

    const handleInput = (e: Event) => {
        const target = e.currentTarget as HTMLInputElement;
        const newValue = target.value;
        if (local.value === undefined) {
            setInternalValue(newValue);
        }
        updateValidation(newValue);
        local.onInput?.(e);
    };

    const handleBlur = (e: Event) => {
        setTouched(true);
        updateValidation(effectiveValue());
        local.onBlur?.(e);
    };

    const classList = createMemo(() => [
        BASE_CLASS,
        local.disabled && variantClasses.disabled,
        (touched() && effectiveError()) && variantClasses.error,
        local.class,
    ].filter(Boolean).join(' '));

    return (
        <div class={classList()}>
            {local.label && <label>{local.label}</label>}
            <input
                type={local.type || 'text'}
                placeholder={local.placeholder}
                disabled={local.disabled}
                value={effectiveValue()}
                name={local.name}
                onInput={handleInput}
                onBlur={handleBlur}
                {...others}
            />
            {touched() && effectiveError() && (
                <Caption class={styles['text-field-error-caption']}>{effectiveError()}</Caption>
            )}
        </div>
    );
}

export default TextField;