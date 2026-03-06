import { createSignal, createEffect, splitProps, JSX, createMemo } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './Checkbox.module.css';

export type CheckboxProps = {
  /** Controlled checked state */
  checked?: boolean;
  /** Uncontrolled initial checked state */
  defaultChecked?: boolean;
  /** Label text */
  label?: string;
  /** Error message from outside (overrides internal error) */
  error?: string;
  /** Callback when checked changes */
  onChange?: (checked: boolean, e: Event) => void;
  /** Callback when checkbox loses focus */
  onBlur?: (e: Event) => void;
  /** Callback when internal error changes */
  onError?: (error: string | null) => void;
  /** Mark as required (must be checked) */
  required?: boolean;
  /** Custom validator (return error message or null) */
  validate?: (checked: boolean) => string | null;
  /** Disabled state */
  disabled?: boolean;
  /** Additional class name for the container */
  class?: string;
} & Omit<JSX.InputHTMLAttributes<HTMLInputElement>, 'type' | 'checked' | 'defaultChecked' | 'onChange' | 'onBlur'>;

export function Checkbox(props: CheckboxProps) {
  const [local, others] = splitProps(props, [
    'checked', 'defaultChecked', 'label', 'error', 'onChange', 'onBlur', 'onError',
    'required', 'validate', 'disabled', 'class'
  ]);

  const [internalChecked, setInternalChecked] = createSignal(local.defaultChecked ?? false);
  const [internalError, setInternalError] = createSignal<string | null>(null);
  const [touched, setTouched] = createSignal(false);

  const effectiveChecked = () => local.checked !== undefined ? local.checked : internalChecked();
  const effectiveError = () => local.error !== undefined ? local.error : internalError();

  const validateChecked = (checked: boolean): string | null => {
    if (local.required && !checked) return 'This field is required';
    if (local.validate) return local.validate(checked);
    return null;
  };

  const updateValidation = (checked: boolean) => {
    const err = validateChecked(checked);
    setInternalError(err);
    local.onError?.(err);
  };

  createEffect(() => {
    if (local.checked !== undefined) {
      updateValidation(local.checked);
    }
  });

  const handleChange = (e: Event) => {
    const target = e.currentTarget as HTMLInputElement;
    const newChecked = target.checked;

    if (local.checked === undefined) {
      setInternalChecked(newChecked);
    }

    updateValidation(newChecked);
    local.onChange?.(newChecked, e);
  };

  const handleBlur = (e: Event) => {
    setTouched(true);
    updateValidation(effectiveChecked());
    local.onBlur?.(e);
  };

  const containerClass = createMemo(() => [
    styles.checkbox,
    local.class,
    local.disabled && styles['checkbox-disabled'],
    touched() && effectiveError() && styles['checkbox-error'],
  ].filter(Boolean).join(' '));

  return (
    <div class={containerClass()}>
      <label class={styles.checkboxLabel}>
        <input
          type="checkbox"
          checked={effectiveChecked()}
          onInput={handleChange}
          onBlur={handleBlur}
          disabled={local.disabled}
          {...others}
        />
        {local.label && <span class={styles.checkboxText}>{local.label}</span>}
      </label>
      {touched() && effectiveError() && (
        <Caption class={styles['checkbox-error-caption']}>{effectiveError()}</Caption>
      )}
    </div>
  );
}