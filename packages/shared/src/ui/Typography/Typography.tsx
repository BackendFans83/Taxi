import { splitProps, ComponentProps, createMemo } from 'solid-js';
import { Dynamic } from 'solid-js/web';
import styles from './Typography.module.css';

export type TypographyVariant = 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'caption' | 'overline' | 'p';

const variantsTags = {
    p: 'p',
    h1: 'h1',
    h2: 'h2',
    h3: 'h3',
    h4: 'h4',
    h5: 'h5',
    h6: 'h6',
    caption: 'p',
    overline: 'p',
} as const;

const variantClasses = {
    p: styles['typography-body'],
    h1: styles['typography-h1'],
    h2: styles['typography-h2'],
    h3: styles['typography-h3'],
    h4: styles['typography-h4'],
    h5: styles['typography-h5'],
    h6: styles['typography-h6'],
    caption: styles['typography-caption'],
    overline: styles['typography-overline'],
} as const;

type TagForVariant<T extends TypographyVariant> = typeof variantsTags[T];

export type TypographyProps<T extends TypographyVariant = 'p'> = {
    variant?: T;
    class?: string;
} & ComponentProps<TagForVariant<T>>;

function Typography<T extends TypographyVariant = 'p'>(props: TypographyProps<T>) {
    const [local, others] = splitProps(props, ['variant', 'class']);
    const variantKey = local.variant ?? 'p';

    const classList = createMemo(() => [
        variantClasses[variantKey],
        local.class,
    ].filter(Boolean).join(' '));

    return (
        <Dynamic
            component={variantsTags[variantKey]}
            class={classList()}
            {...(others as any)}
        />
    );
}

Typography.displayName = 'Typography';

function createTypoComponent<T extends TypographyVariant>(variant: T) {
    type Props = ComponentProps<TagForVariant<T>>;
    const Comp = (props: Props) => (
        <Typography<T> {...props} variant={variant} />
    );
    Comp.displayName = `Typography.${variant.toUpperCase()}`;
    return Comp;
}

export const P = createTypoComponent('p');
export const H1 = createTypoComponent('h1');
export const H2 = createTypoComponent('h2');
export const H3 = createTypoComponent('h3');
export const H4 = createTypoComponent('h4');
export const H5 = createTypoComponent('h5');
export const H6 = createTypoComponent('h6');
export const Caption = createTypoComponent('caption');
export const Overline = createTypoComponent('overline');

export default Typography;