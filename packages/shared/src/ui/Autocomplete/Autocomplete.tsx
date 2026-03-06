import { createSignal, createEffect, splitProps, JSX, For, onCleanup, createMemo } from 'solid-js';
import { Caption } from '../Typography/Typography';
import styles from './Autocomplete.module.css';

export type AutocompleteOption = {
    value: string;
    label: string;
    icon?: JSX.Element;
};

export type AutocompleteProps = {
    /** Controlled input value */
    value?: string;
    /** Uncontrolled initial input value */
    defaultValue?: string;
    /** Array of options to display */
    options: AutocompleteOption[];
    /** Label for the input */
    label?: string;
    /** Placeholder text */
    placeholder?: string;
    /** Error message from outside (overrides internal error) */
    error?: string;
    /** Callback when input value changes */
    onInput?: (value: string, e: Event) => void;
    /** Callback when an option is selected */
    onSelect?: (option: AutocompleteOption) => void;
    /** Callback when input loses focus */
    onBlur?: (e: Event) => void;
    /** Callback when internal error changes */
    onError?: (error: string | null) => void;
    /** Mark as required (for validation) */
    required?: boolean;
    /** Custom validator (receives current input value, returns error message or null) */
    validate?: (value: string) => string | null;
    /** Disabled state */
    disabled?: boolean;
    /** Name attribute for the input */
    name?: string;
    /** Additional class name for the wrapper */
    class?: string;
    /** Function to filter options based on input value (default: case-insensitive includes) */
    filterOptions?: (option: AutocompleteOption, inputValue: string) => boolean;
} & Omit<JSX.InputHTMLAttributes<HTMLInputElement>, 'onInput' | 'onBlur' | 'value' | 'defaultValue'>;

export function Autocomplete(props: AutocompleteProps) {
    const [local, others] = splitProps(props, [
        'value', 'defaultValue', 'options', 'label', 'placeholder', 'error',
        'onInput', 'onSelect', 'onBlur', 'onError', 'required', 'validate',
        'disabled', 'name', 'class', 'filterOptions'
    ]);

    const [inputValue, setInputValue] = createSignal(local.defaultValue ?? '');
    const [internalError, setInternalError] = createSignal<string | null>(null);
    const [touched, setTouched] = createSignal(false);
    const [isOpen, setIsOpen] = createSignal(false);
    const [highlightedIndex, setHighlightedIndex] = createSignal(-1);

    let inputRef: HTMLInputElement | undefined;

    const effectiveValue = () => local.value !== undefined ? local.value : inputValue();
    const effectiveError = () => local.error !== undefined ? local.error : internalError();

    const defaultFilter = (option: AutocompleteOption, val: string) =>
        option.label.toLowerCase().includes(val.toLowerCase());

    const filterFn = local.filterOptions ?? defaultFilter;

    const filteredOptions = () =>
        effectiveValue().trim() === ''
            ? local.options
            : local.options.filter(opt => filterFn(opt, effectiveValue()));

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

    const handleClickOutside = (e: MouseEvent) => {
        if (inputRef && !inputRef.contains(e.target as Node)) {
            setIsOpen(false);
        }
    };

    createEffect(() => {
        if (isOpen()) {
            document.addEventListener('click', handleClickOutside);
        } else {
            document.removeEventListener('click', handleClickOutside);
        }
        onCleanup(() => document.removeEventListener('click', handleClickOutside));
    });

    const handleInput = (e: Event) => {
        const target = e.currentTarget as HTMLInputElement;
        const newValue = target.value;

        if (local.value === undefined) {
            setInputValue(newValue);
        }

        updateValidation(newValue);
        setIsOpen(true);
        setHighlightedIndex(-1);
        local.onInput?.(newValue, e);
    };

    const handleOptionClick = (option: AutocompleteOption) => {
        if (local.value === undefined) {
            setInputValue(option.label);
        }
        updateValidation(option.label);
        setIsOpen(false);
        local.onSelect?.(option);
        // Trigger onInput to propagate the change
        const fakeEvent = new Event('input', { bubbles: true });
        if (inputRef) {
            inputRef.value = option.label;
            inputRef.dispatchEvent(fakeEvent);
        }
    };

    const handleBlur = (e: Event) => {
        setTouched(true);
        updateValidation(effectiveValue());
        // Delay closing to allow option click to register
        setTimeout(() => {
            if (!inputRef?.matches(':focus')) {
                setIsOpen(false);
            }
        }, 200);
        local.onBlur?.(e);
    };

    const handleKeyDown = (e: KeyboardEvent) => {
        if (!isOpen()) {
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                setIsOpen(true);
            }
            return;
        }

        const filtered = filteredOptions();
        if (filtered.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                setHighlightedIndex(prev =>
                    prev < filtered.length - 1 ? prev + 1 : prev
                );
                break;
            case 'ArrowUp':
                e.preventDefault();
                setHighlightedIndex(prev => (prev > 0 ? prev - 1 : -1));
                break;
            case 'Enter':
                e.preventDefault();
                if (highlightedIndex() >= 0 && highlightedIndex() < filtered.length) {
                    handleOptionClick(filtered[highlightedIndex()]);
                }
                break;
            case 'Escape':
                setIsOpen(false);
                setHighlightedIndex(-1);
                break;
        }
    };

    const containerClass = createMemo(() => [
        styles.autocomplete,
        local.class,
        local.disabled && styles['autocomplete-disabled'],
        touched() && effectiveError() && styles['autocomplete-error'],
    ].filter(Boolean).join(' '));

    return (
        <div class={containerClass()} ref={inputRef}>
            {local.label && <label class={styles['autocomplete-label']}>{local.label}</label>}
            <input
                type="text"
                class={styles['autocomplete-input']}
                value={effectiveValue()}
                onInput={handleInput}
                onBlur={handleBlur}
                onKeyDown={handleKeyDown}
                onFocus={() => setIsOpen(true)}
                placeholder={local.placeholder}
                disabled={local.disabled}
                name={local.name}
                {...others}
            />
            {isOpen() && filteredOptions().length > 0 && (
                <div class={styles['autocomplete-options']}>
                    <For each={filteredOptions()}>
                        {(option, index) => (
                            <div
                                class={`${styles['autocomplete-option']} ${index() === highlightedIndex() ? styles['autocomplete-option-highlighted'] : ''
                                    }`}
                                onClick={() => handleOptionClick(option)}
                                onMouseEnter={() => setHighlightedIndex(index())}
                            >
                                {option.icon && <span class={styles['autocomplete-option-icon']}>{option.icon}</span>}
                                <span>{option.label}</span>
                            </div>
                        )}
                    </For>
                </div>
            )}
            {touched() && effectiveError() && (
                <Caption class={styles['autocomplete-error-caption']}>{effectiveError()}</Caption>
            )}
        </div>
    );
}