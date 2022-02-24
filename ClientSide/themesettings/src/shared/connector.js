export default {

    currentCulture: "fr-FR",
    connect() {

    },
    loadAll: function (resources, onSuccess) {
        onSuccess();
    },
    getMessage() {

    },
    /**
         * Loads data source (value/text) pairs from a remote source.
         * This default implementation allows for config to be a string identifying a URL.
         *
         * @param config
         * @param successCallback
         * @param errorCallback
         * @returns {*}
         */
    loadDataSource: function (config, successCallback, errorCallback) {
        //return this._handleLoadDataSource(config, successCallback, errorCallback);

        console.log("loadDataSource");
        console.log(config);

        if (config && config.query && config.query) {
            if (config.query.type == "folders") {
                successCallback([{ id: "1", name: "Files", url: "/Files" }]);
            }

            if (config.query.type == "files") {
                var files = [{ id: "1", url: "https://agontuk.github.io/assets/images/berserk.jpg", name: "berserk.jpg", folderId: "1" }];
                successCallback(files.filter((f) => {
                    if (config.query.folder)
                        return f.folderId == config.query.folder;
                    else
                        return false;
                }).map(f => {
                    return {
                        id: f.id,
                        filename: f.name,
                        url: f.url
                    };
                }));
            }
        }
        else {
            errorCallback();
        }


    },
    // eslint-disable-next-line no-unused-vars
    upload(config, successCallback, errorCallback) {
        //debugger;


        var uploadImage = function (name, file, callbackFn) {
            var formData = new FormData();
            formData.append('file', file);

            // eslint-disable-next-line no-undef
            $.ajax({
                type: 'POST',
                // eslint-disable-next-line no-undef
                url: abp.appPath + 'api/app/cms/upload' + abp.utils.buildQueryString([{ name: 'name', value: name }]) + '',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    callbackFn(response);
                },
            });
        };
        uploadImage(config.file.name || config.name, config.file, function (data) {

            successCallback({ id: data.id, url: data.url, filename: data.filename });
        });

      

    }
}
