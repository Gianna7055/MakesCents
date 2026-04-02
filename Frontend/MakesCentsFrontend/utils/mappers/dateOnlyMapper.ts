import { DateOnly } from "@/types/date-only";

export const toDateOnly = (date: Date): DateOnly => {
  const d = new DateOnly();
  console.log("In toDateOnly");
  d.year = date.getFullYear();
  d.month = date.getMonth() + 1; // JS months are 0-based
  d.day = date.getDate();

  return d;
};

export const fromDateOnly = (date: DateOnly): Date => {
  return new Date(date.year, date.month - 1, date.day);
};
