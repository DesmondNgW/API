'use strict';
import "./extend/json.js";
import { cache } from "./cache.js";
import SHA1 from "crypto-js/sha1";

export const util = {
    /**
     * getConsistentHash
     * @param {any} nodes
     * @param {any} key
     */
    getConsistentHash: function (nodes, key) {
        if (nodes.length) return "";
        let cacheKey = SHA1(nodes.toJson());
        let circle = cache.get(cacheKey);
        if (!circle) {
            circle = {};
            const virtualNodeCount = 150;
            for (let i = 0, len = nodes.length, item = nodes[i]; i < len; i++) {
                for (let j = 0; j < virtualNodeCount; j++) {
                    circle[SHA1(item + "_clone_" + j)] = item;
                }
            }
            cache.set(cacheKey, circle);
        }
        let sha1Key = SHA1(key);
        let keys = Object.keys(circle);
        var ret = keys.fiter((value) => { return value == sha1Key; });
        if (ret && ret.length) {
            return ret[0];
        }
        var keySort = keys.sort();
        if (keySort && keySort.length) {
            return circle[keySort[0]]
        }
        return "";
    }
}