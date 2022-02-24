<template>
    <div class="hello">
        <h4>{{ title }}</h4>
        <lama-form v-model="model" v-bind="def"></lama-form>
    </div>
</template>

<script>
    import { LamaForm } from "lamavue";
    import Connector from "../shared/connector";
    export default {
        name: 'SettingsForm',
        props: {
            title: String,
            value: Object,
            schema: Object,
            options: Object
        },
        data() {
            return {
                connector: Connector,
            };
        },
        computed: {
            def() {
                return {
                    schema: this.schema || {},
                    options: this.options || {},
                    view: "bootstrap4-create",
                    connector: this.connector
                }
            },
            locale() {
                return this.connector.locale();
            },
            model: {
                get() {
                    return this.value;
                },
                set(val) {
                    this.$emit("input", val);
                }
            }
        },
        components: {
            LamaForm
        }
       
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    h3 {
        margin: 40px 0 0;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        display: inline-block;
        margin: 0 10px;
    }

    a {
        color: #42b983;
    }
</style>
