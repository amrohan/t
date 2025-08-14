import DefaultTheme from "vitepress/theme";
import Contributors from "../components/Contributors.vue";
import VideoPlayer from "../components/VideoPlayer.vue";

import "./custom.css";

export default {
  extends: DefaultTheme,
  enhanceApp({ app }) {
    app.component("Contributors", Contributors);
    app.component("VideoPlayer", VideoPlayer);
  },
};
