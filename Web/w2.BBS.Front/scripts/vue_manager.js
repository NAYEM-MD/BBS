// (c) 2025 W2 Co.,Ltd.

// ルートVue管理
class VueManager {
  constructor() {
    this.pluginNames = [];
    this.vueApp = null;
    this.vm = null;
    this.emitter = null;
    this.word = null;
    this.frontSetting = null;
    this.validatorSetting = null;
    this.init();
  }
  // 初期化
  init() {
    const app = {
      mounted() {
        console.log('mounted');
      },
      watch: {
      },
      methods: {
      },
      computed: {
      },
      components: {
      },
      created() {
        console.log('created');
      },
      compilerOptions: {
        delimiters: ['${', '}'],
        comments: true,
        isCustomElement: (tag) => {
          return tag.startsWith('v-')
        }
      }
    };
    this.vueApp = Vue.createApp(app);
    this.vueApp.use(Vuetify.createVuetify());

    this.emitter = mitt();
  }
  // Vueのマウント
  mount() {
    this.vm = this.vueApp.mount('#rootTemplate');
  }
  // プラグイン設定
  setPlugin(plugin, pluginName, options) {
    this.vueApp.use(plugin, options);
    if (this.checkInstallPlugin(pluginName) === false) {
      this.pluginNames.push(pluginName);
      console.log(pluginName + 'プラグインが追加されました。');
    }
    console.log(pluginName + 'プラグインの実行が登録されました。');
  }
  /**
   * コンポーネント追加
   * @param {any} componentName コンポーネント名
   * @param {any} content コンポーネント内容
   */
  setComponent(componentName, content) {
    content.compilerOptions = {
      delimiters: ['${', '}'],
      comments: true
    };
    this.vueApp.component(componentName, content);
  }
  /**
   * プラグインの導入チェック
   * @param {any} pluginName プラグイン名
   */
  checkInstallPlugin(pluginName) {
    return this.pluginNames.includes(pluginName);
  }
  /**
   * バリデータカスタムディレクティブ追加
   * @param {any} validatorSetting バリデータ設定
   */
  setValidatorDirective(validatorSetting) {
    this.validatorSetting = validatorSetting;
    const myClass = this;

    this.vueApp.directive('validator', {
      mounted(el, binding, vnode) {
        if (el.type == "password") {
          // type=passwordはセキュリティの観点でchangeイベントが走らないのでinputイベントを追加
          el.addEventListener('input', function (e) {
            lib.utility.Validator.validateSingle([binding.value, $(e.target)[0].value, myClass.validatorSetting, el]);
          });
        } else {
          // changeイベントを追加
          el.addEventListener('change', function (e) {
            lib.utility.Validator.validateSingle([binding.value, $(e.target)[0].value, myClass.validatorSetting, el]);
          });
        }
      },
    });
    console.log('v-validatorディレクティブが登録されました。');
  }
}
