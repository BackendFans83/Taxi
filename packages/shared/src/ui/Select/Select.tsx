import { createSignal, createEffect, splitProps, JSX, createMemo } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './Select.module.css';

export type SelectOption = {
  value: string;
  label: string;
  disabled?: boolean;
};

export type SelectProps = {
  /** Controlled value */
  value?: string;
  /** Uncontrolled initial value */
  defaultValue?: string;
  /** Label text */
  label?: string;
  /** Error message from outside (overrides internal error) */
  error?: string;
  /** Callback when value changes */
  onChange?: (e: Event) => void;
  /** Callback when select loses focus */
  onBlur?: (e: Event) => void;
  /** Callback when internal error changes */
  onError?: (error: string | null) => void;
  /** Mark field as required */
  required?: boolean;
  /** Custom validator (return error message or null) */
  validate?: (value: string) => string | null;
  /** Array of options */
  options?: SelectOption[];
  /** Disabled state */
  disabled?: boolean;
  /** Additional class names */
  class?: string;
} & Omit<JSX.SelectHTMLAttributes<HTMLSelectElement>, 'onChange' | 'onBlur' | 'value' | 'defaultValue'>;

const BASE_CLASS = styles.select;

export function Select(props: SelectProps) {
  const [local, others] = splitProps(props, [
    'value', 'defaultValue', 'label', 'error', 'onChange', 'onBlur', 'onError',
    'required', 'validate', 'options', 'disabled', 'class', 'children'
  ]);

  const [internalValue, setInternalValue] = createSignal(local.defaultValue ?? '');
  const [internalError, setInternalError] = createSignal<string | null>(null);
  const [touched, setTouched] = createSignal(false);

  const effectiveValue = () => local.value !== undefined ? local.value : internalValue();
  const effectiveError = () => local.error !== undefined ? local.error : internalError();

  const validateValue = (val: string): string | null => {
    if (local.required && !val) return 'This field is required';
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

  const handleChange = (e: Event) => {
    const target = e.currentTarget as HTMLSelectElement;
    const newValue = target.value;

    if (local.value === undefined) {
      setInternalValue(newValue);
    }

    updateValidation(newValue);
    local.onChange?.(e);
  };

  const handleBlur = (e: Event) => {
    setTouched(true);
    updateValidation(effectiveValue());
    local.onBlur?.(e);
  };

  const containerClass = createMemo(() =>
    [
      BASE_CLASS,
      local.class,
      local.disabled && styles['select-disabled'],
      touched() && effectiveError() && styles['select-error'],
    ]
    .filter(Boolean)
    .join(' ')
  );

  return (
    <div class={containerClass()}>
      {local.label && <label>{local.label}</label>}
      <select
        {...others}
        value={effectiveValue()}
        onInput={handleChange}
        onBlur={handleBlur}
        disabled={local.disabled}
      >
        {local.options
          ? local.options.map(opt => (
              <option value={opt.value} disabled={opt.disabled}>
                {opt.label}
              </option>
            ))
          : local.children}
      </select>
      {touched() && effectiveError() && (
        <Caption class={styles['select-error-caption']}>{effectiveError()}</Caption>
      )}
    </div>
  );
}

export default Select;