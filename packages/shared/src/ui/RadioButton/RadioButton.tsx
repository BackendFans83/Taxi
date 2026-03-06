import { createSignal, createEffect, splitProps, createMemo, For } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './RadioButton.module.css';

export type RadioOption = {
  value: string;
  label: string;
  disabled?: boolean;
};

export type RadioGroupProps = {
  /** Controlled value */
  value?: string;
  /** Uncontrolled initial value */
  defaultValue?: string;
  /** Name attribute for all radio inputs (automatically generated if not provided) */
  name?: string;
  /** Array of options */
  options: RadioOption[];
  /** Label for the group */
  label?: string;
  /** Error message from outside (overrides internal error) */
  error?: string;
  /** Callback when value changes */
  onChange?: (value: string, e: Event) => void;
  /** Callback when group loses focus (when any radio blurs) */
  onBlur?: (e: Event) => void;
  /** Callback when internal error changes */
  onError?: (error: string | null) => void;
  /** Mark group as required */
  required?: boolean;
  /** Custom validator (receives current value, returns error message or null) */
  validate?: (value: string) => string | null;
  /** Disable all radios in group */
  disabled?: boolean;
  /** Additional class name for the container */
  class?: string;
};

export function RadioGroup(props: RadioGroupProps) {
  const [local, others] = splitProps(props, [
    'value', 'defaultValue', 'name', 'options', 'label', 'error',
    'onChange', 'onBlur', 'onError', 'required', 'validate', 'disabled', 'class'
  ]);

  const [generatedName] = createSignal(`radio-${Math.random().toString(36).substr(2, 9)}`);
  const name = () => local.name ?? generatedName();

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
    const target = e.currentTarget as HTMLInputElement;
    const newValue = target.value;

    if (local.value === undefined) {
      setInternalValue(newValue);
    }

    updateValidation(newValue);
    local.onChange?.(newValue, e);
  };

  const handleBlur = (e: Event) => {
    setTouched(true);
    updateValidation(effectiveValue());
    local.onBlur?.(e);
  };

  const containerClass = createMemo(() => 
    [
      styles['radio-group'],
      local.class,
      local.disabled && styles['radio-group-disabled'],
      touched() && effectiveError() && styles['radio-group-error'],
    ]
      .filter(Boolean)
      .join(' ')
  );

  return (
    <div class={containerClass()} {...others}>
      {local.label && <div class={styles['radio-group-label']}>{local.label}</div>}
      <div class={styles['radio-group-options']}>
        <For each={local.options}>
          {(option) => (
            <label class={styles['radio-option']}>
              <input
                type="radio"
                name={name()}
                value={option.value}
                checked={effectiveValue() === option.value}
                onChange={handleChange}
                onBlur={handleBlur}
                disabled={local.disabled || option.disabled}
              />
              <span class={styles['radio-control']} aria-hidden="true" />
              <span class={styles['radio-label']}>{option.label}</span>
            </label>
          )}
        </For>
      </div>
      {touched() && effectiveError() && (
        <Caption class={styles['radio-group-error-caption']}>{effectiveError()}</Caption>
      )}
    </div>
  );
}