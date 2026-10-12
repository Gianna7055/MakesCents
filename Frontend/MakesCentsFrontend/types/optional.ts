/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { IOptional } from "./i-optional";

/**
 * Optional class for creating and updating nulls
 * 
 * @typeParam T -
 */
export class Optional<T> implements IOptional {
    /**
     * Checks if the value was sent
     */
    hasValue: boolean;
    /**
     * The value that was sent
     */
    value: T | null;
}
