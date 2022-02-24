<template>
    <div id="app">
        <div class="themes-app personaBar-mainContainer">
            <div class="themes-Root">
                <div class="dnn-persona-bar-page show  undefined">
                    <div class="dnn-grid-cell themes-body" style="width: 100%;">
                        <div class="dnn-persona-bar-page-header">
                            <span class="title">
                                <h3>Theme Settings</h3>
                            </span>
                            <div class="children">
                                <button class="dnn-ui-common-button large" role="primary" @click="save">Save</button>
                            </div>
                        </div>
                        <div class="dnn-grid-cell dnn-persona-bar-page-body" style="width: 100%;">
                            <div class="dnn-grid-cell persona-bar-page-body" style="width: 100%;">
                                <div class="dnn-grid-cell site-theme" style="width: 100%;">
                                    <div v-if="message" style="margin:10px;border:solid red 1px;padding:10px;">{{message}}</div>
                                    <SettingsForm v-if="global.exist" v-model="global.model" :schema="global.schema" :options="global.options" title="Global Style Settings (skin.css)" />
                                    <hr />
                                    <SettingsForm v-if="skin.exist" v-model="skin.model" :schema="skin.schema" :options="skin.options" :title="'Layout Settings ('+file+' + css)'" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script>
    import SettingsForm from './components/SettingsForm.vue'
    import ThemeService from "./themeSettingsService";

    export default {
        name: 'App',
        data() {
            return {
                file: '',
                message: '',
                global: {
                    exist: false,
                    model: {},
                    schema: {},
                    options: {}
                },
                skin: {
                    exist: false,
                    model: {},
                    schema: {},
                    options: {}
                }
            };
        },
        components: {
            SettingsForm
        },
        methods: {
            save() {
                ThemeService.saveSettings(this.global.model, this.skin.model, data => {
                    console.log(data);
                    if (data.Succes) {
                        this.message = 'Saved with success.';
                        //window.parent.location.reload();
                    }
                    else {
                        //alert(data.Message);
                        this.message = data.Message;
                    }
                }, this.errorCallback);
            },
            errorCallback(xhr) {
                let response = eval("(" + xhr.responseText + ")");
                let message = xhr.responseText;
                if (response && response.Message) {
                    message = response.Message;
                }
                alert(message);
                //let utils = window.dnn.initThemes().utility;
                //utils.notifyError(Localization.get(message), 5000);
            }
        },
        created() {
            ThemeService.getSettings(data => {
                console.log(data);
                this.file = data.File;
                this.global = {
                    exist: data.Global.Exist,
                    schema: JSON.parse(data.Global.Schema),
                    options: JSON.parse(data.Global.Options),
                    model: JSON.parse(data.Global.Settings)
                };
                this.skin = {
                    exist: data.Skin.Exist,
                    schema: JSON.parse(data.Skin.Schema),
                    options: JSON.parse(data.Skin.Options),
                    model: JSON.parse(data.Skin.Settings)
                };
            }, this.errorCallback);
        }
    }
</script>

<style>
    #app {
    }

    .site-theme {
        border: 1px solid #1E88C3;
        padding: 30px 22px;
        min-height: 235px;
        background-color: #FFFFFF;
    }
</style>
