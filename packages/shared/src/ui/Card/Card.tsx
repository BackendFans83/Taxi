import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Card.module.css';

// ----- Card root -----
export type CardProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Card(props: CardProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [styles.card, local.class].filter(Boolean).join(' '));
  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}

// ----- CardMedia -----
export type CardMediaProps = {
  src: string;
  alt?: string;
  class?: string;
} & JSX.ImgHTMLAttributes<HTMLImageElement>;

export function CardMedia(props: CardMediaProps) {
  const [local, others] = splitProps(props, ['src', 'alt', 'class']);
  const classList = createMemo(() => [styles['card-media'], local.class].filter(Boolean).join(' '));
  return (
    <img src={local.src} alt={local.alt} class={classList()} {...others} />
  );
}

// ----- CardContent -----
export type CardContentProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function CardContent(props: CardContentProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [styles['card-content'], local.class].filter(Boolean).join(' '));
  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}

// ----- CardActions -----
export type CardActionsProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function CardActions(props: CardActionsProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [styles['card-actions'], local.class].filter(Boolean).join(' '));
  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}