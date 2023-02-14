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
                                <button class="dnn-ui-common-button large" role="button" @click="save">Save</button>
                                <button role="button" @click.prevent="showBuilder=!showBuilder" class="dnn-ui-common-button large" style="margin-right: 20px; color: #808080; border-color: #ffffff;">{{showBuilder?'Show Settings': 'Show Form builder'}}</button>
                            </div>
                        </div>
                        <div v-if="!showBuilder" class="dnn-grid-cell dnn-persona-bar-page-body" style="width: 100%;">
                            <div class="dnn-grid-cell persona-bar-page-body" style="width: 100%;">
                                <div class="dnn-grid-cell site-theme" style="width: 100%;">
                                    <div style="text-align:right;">
                                        <span v-if="message" style="margin:10px;background-color:green;color:#ffffff;padding:10px;">{{message}}</span>
                                    </div>
                                    <SettingsForm v-if="global.exist" v-model="global.model" :schema="global.schema" :options="global.options" title="Global Style Settings (skin.css)" />
                                    <hr />
                                    <SettingsForm v-if="skin.exist" v-model="skin.model" :schema="skin.schema" :options="skin.options" :title="'Layout Settings ('+file+' + css)'" />
                                </div>
                            </div>
                        </div>
                        <div v-else class="dnn-grid-cell dnn-persona-bar-page-body" style="width: 100%;">
                            <div class="dnn-grid-cell persona-bar-page-body" style="width: 100%;">
                                <div class="dnn-grid-cell site-theme" style="width: 100%;">
                                    <div style="text-align:right;">
                                        <span v-if="message" style="margin:10px;background-color:green;color:#ffffff;padding:10px;">{{message}}</span>
                                    </div>
                                    <BuilderForm v-if="global.exist" v-model="global.builder"  title="Global Style Settings (skin.css)" />
                                    <hr />
                                    <BuilderForm v-if="skin.exist" v-model="skin.builder"  :title="'Layout Settings ('+file+' + css)'" />
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
    import BuilderForm from './components/BuilderForm.vue'
    import ThemeService from "./themeSettingsService";

    export default {
        name: 'App',
        data() {
            return {
                showBuilder: false,
                file: '',
                message: '',
                global: {
                    exist: false,
                    model: {},
                    schema: {},
                    options: {},
                    builder: {
                        schema: {
                            type: "object",
                            properties: {},
                        },
                        options: { fields: {} },
                    }
                },
                skin: {
                    exist: false,
                    model: {},
                    schema: {},
                    options: {},
                    builder: {
                        schema: {
                            type: "object",
                            properties: {},
                        },
                        options: { fields: {} },
                    }
                }
            };
        },
        components: {
            SettingsForm,
            BuilderForm
        },
        methods: {
            save() {
                if (this.showBuilder) {
                    ThemeService.saveBuilder(this.global.builder, this.skin.builder, data => {
                        //console.log(data);
                        if (data.Succes) {
                            this.message = 'Saved with success.';
                            //window.parent.location.reload();
                            this.fetchSettings();
                        }
                        else {
                            //alert(data.Message);
                            this.message = data.Message;
                        }
                    }, this.errorCallback);
                } else {
                    ThemeService.saveSettings(this.global.model, this.skin.model, data => {
                        //console.log(data);
                        if (data.Succes) {
                            this.message = 'Saved with success.';
                            //window.parent.location.reload();
                            
                        }
                        else {
                            //alert(data.Message);
                            this.message = data.Message;
                        }
                    }, this.errorCallback);
                }
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
            },
            fetchSettings() {
                ThemeService.getSettings(data => {
                    console.log(data);
                    this.file = data.File;
                    this.global = {
                        exist: data.Global.Exist,
                        schema: JSON.parse(data.Global.Schema),
                        options: JSON.parse(data.Global.Options),
                        model: JSON.parse(data.Global.Settings),
                        builder: {}
                    };
                    this.global.builder = {
                        schema: this.global.schema,
                        options: this.global.options
                    };
                    this.skin = {
                        exist: data.Skin.Exist,
                        schema: JSON.parse(data.Skin.Schema),
                        options: JSON.parse(data.Skin.Options),
                        model: JSON.parse(data.Skin.Settings),
                        builder: {}
                    };
                    this.skin.builder = {
                        schema: this.skin.schema,
                        options: this.skin.options
                    };
                }, this.errorCallback);
            }
        },
        created() {
            this.fetchSettings();
        },
        computed: {

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
