import { createSignal, createEffect, splitProps, JSX, createMemo } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './Switch.module.css';

export type SwitchProps = {
  /** Controlled checked state */
  checked?: boolean;
  /** Uncontrolled initial checked state */
  defaultChecked?: boolean;
  /** Label text displayed to the right of the switch */
  label?: string;
  /** Error message from outside (overrides internal error) */
  error?: string;
  /** Callback when checked changes */
  onChange?: (checked: boolean, e: Event) => void;
  /** Callback when switch loses focus */
  onBlur?: (e: Event) => void;
  /** Callback when internal error changes */
  onError?: (error: string | null) => void;
  /** Mark as required (for validation) */
  required?: boolean;
  /** Custom validator (receives current checked value, returns error message or null) */
  validate?: (checked: boolean) => string | null;
  /** Disabled state */
  disabled?: boolean;
  /** Name attribute for the input */
  name?: string;
  /** Value attribute for the input (useful in forms) */
  value?: string;
  /** Additional class name for the wrapper */
  class?: string;
} & Omit<JSX.InputHTMLAttributes<HTMLInputElement>, 'onChange' | 'onBlur' | 'checked' | 'defaultChecked'>;

export function Switch(props: SwitchProps) {
  const [local, others] = splitProps(props, [
    'checked', 'defaultChecked', 'label', 'error', 'onChange', 'onBlur', 'onError',
    'required', 'validate', 'disabled', 'name', 'value', 'class'
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

  const wrapperClass = createMemo(() => [
      styles['switch-wrapper'],
      local.class,
      local.disabled && styles['switch-wrapper-disabled'],
      touched() && effectiveError() && styles['switch-wrapper-error'],
    ]
      .filter(Boolean)
      .join(' ')
  );

  return (
    <div class={wrapperClass()}>
      <label class={styles.switch}>
        <input
          type="checkbox"
          class={styles['switch-input']}
          checked={effectiveChecked()}
          onChange={handleChange}
          onBlur={handleBlur}
          disabled={local.disabled}
          name={local.name}
          value={local.value}
          {...others}
        />
        <span class={styles['switch-slider']} />
      </label>
      {local.label && <span class={styles['switch-label']}>{local.label}</span>}
      {touched() && effectiveError() && (
        <Caption class={styles['switch-error-caption']}>{effectiveError()}</Caption>
      )}
    </div>
  );
}