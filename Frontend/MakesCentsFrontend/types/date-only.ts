/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { IUtf8SpanFormattable } from "./i-utf8-span-formattable";
import { DayOfWeek } from "./day-of-week";

export class DateOnly implements IUtf8SpanFormattable {
    static minValue: DateOnly;
    static maxValue: DateOnly = "9999-12-31";
    year: number = 1;
    month: number = 1;
    day: number = 1;
    dayOfWeek: DayOfWeek = 1;
    dayOfYear: number = 1;
    dayNumber: number;
}
