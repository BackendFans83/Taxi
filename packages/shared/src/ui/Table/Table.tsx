import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Table.module.css';

// ----- TableContainer (div wrapper) -----
export type TableContainerProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function TableContainer(props: TableContainerProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [
      styles['table-container'],
      local.class
    ]
      .filter(Boolean)
      .join(' ')
  );

  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}

// ----- Table (table) -----
export type TableProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLTableElement>;

export function Table(props: TableProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [
      styles.table,
      local.class
    ]
      .filter(Boolean)
      .join(' ')
  );
  return (
    <table class={classList()} {...others}>
      {local.children}
    </table>
  );
}

// ----- Thead (thead) -----
export type TheadProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLTableSectionElement>;

export function Thead(props: TheadProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  return (
    <thead class={local.class} {...others}>
      {local.children}
    </thead>
  );
}

// ----- Tbody (tbody) -----
export type TbodyProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLTableSectionElement>;

export function Tbody(props: TbodyProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  return (
    <tbody class={local.class} {...others}>
      {local.children}
    </tbody>
  );
}

// ----- Tr (tr) -----
export type TrProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLTableRowElement>;

export function Tr(props: TrProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  return (
    <tr class={local.class} {...others}>
      {local.children}
    </tr>
  );
}

// ----- Th (th) -----
export type ThProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.ThHTMLAttributes<HTMLTableCellElement>;

export function Th(props: ThProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  return (
    <th class={local.class} {...others}>
      {local.children}
    </th>
  );
}

// ----- Td (td) -----
export type TdProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.TdHTMLAttributes<HTMLTableCellElement>;

export function Td(props: TdProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  return (
    <td class={local.class} {...others}>
      {local.children}
    </td>
  );
}