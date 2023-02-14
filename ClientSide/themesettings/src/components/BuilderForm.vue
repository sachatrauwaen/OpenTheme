<template>
    <div class="hello">
        <h4>{{ title }}</h4>
        <builder v-model="model" v-bind="def"></builder>
    </div>
</template>

<script>
    import { Builder } from "lamavue";
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
            Builder
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
