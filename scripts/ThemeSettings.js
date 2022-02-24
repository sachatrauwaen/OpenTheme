define(['jquery', 'main/extension', 'main/config'], function ($, ext, cf) {
    'use strict';
    var identifier;
    var config = cf.init();

    var init = function (wrapper, util, params, callback) {
        identifier = params.identifier;
        window.dnn.initThemeSettings = function () {
            return {
                utility: util,
                params: params,
                moduleName: "ThemeSettings"
            };
        };

        util.loadBundleScript('modules/satrabel.themesettings/scripts/bundles/js/app.js');

        if (typeof callback === 'function') {
            callback();
        }
    };

    var load = function (params, callback) {
        if (typeof callback === 'function') {
            callback();
        }
    };

    return {
        init: init,
        load: load
    };
});
