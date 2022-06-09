import CacheContext from "./cacheContext";

/**
 * CacheContextAttribute
 * @param {any} key
 * @param {any} version 
 * @param {any} date
 * @param {any} callSuccess
 */
export const CacheContextAttribute = function(key, version, date, callSuccess) {
    return (Provider) => {
        return new CacheContext(Provider, {
            Key: key,
            Version: version,
            Date: date,
            CallSuccess: callSuccess
        })
    }
};