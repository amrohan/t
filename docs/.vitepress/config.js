import { defineConfig } from "vitepress";

export default defineConfig({
  title: "Termix",
  description: "A modern, high-performance file navigator for your terminal",
  base: "/",
  head: [
    ["link", { rel: "icon", href: "/favicon.ico" }],
    ["meta", { name: "theme-color", content: "#3c82f6" }],
  ],
  themeConfig: {
    logo: "/logo.svg",
    siteTitle: "Termix",
    nav: [
      { text: "Home", link: "/" },
      { text: "Guide", link: "/guide/introduction" },
      { text: "API Reference", link: "/api/overview" },
      { text: "GitHub", link: "https://github.com/amrohan/termix" },
    ],
    sidebar: {
      "/guide/": [
        {
          text: "Getting Started",
          items: [
            { text: "Introduction", link: "/guide/introduction" },
            { text: "Installation", link: "/guide/installation" },
            { text: "Quick Start", link: "/guide/quick-start" },
          ],
        },
        {
          text: "User Guide",
          items: [
            { text: "Navigation", link: "/guide/navigation" },
            { text: "File Operations", link: "/guide/file-operations" },
            { text: "Search & Filter", link: "/guide/search-filter" },
            { text: "Keyboard Shortcuts", link: "/guide/keyboard-shortcuts" },
          ],
        },
        {
          text: "Advanced",
          items: [
            { text: "Configuration", link: "/guide/configuration" },
            { text: "Tips & Tricks", link: "/guide/tips-tricks" },
            { text: "Troubleshooting", link: "/guide/troubleshooting" },
          ],
        },
      ],
      "/api/": [
        {
          text: "API Reference",
          items: [
            { text: "Overview", link: "/api/overview" },
            { text: "ActionService", link: "/api/action-service" },
            { text: "FileSystemService", link: "/api/filesystem-service" },
            { text: "File Manager", link: "/api/file-manager" },
          ],
        },
        {
          text: "Architecture",
          items: [
            { text: "Core Components", link: "/api/core-components" },
            { text: "Services", link: "/api/services" },
            { text: "UI Layer", link: "/api/ui-layer" },
          ],
        },
      ],
    },
    socialLinks: [
      { icon: "github", link: "https://github.com/amrohan/termix" },
    ],
    footer: {
      message: "Released under the MIT License",
      copyright: "Copyright © 2025 amrohan",
    },
    editLink: {
      pattern: "https://github.com/amrohan/termix/edit/main/docs/:path",
      text: "Edit this page on GitHub",
    },
    search: {
      provider: "local",
    },
    lastUpdated: {
      text: "Updated at",
      formatOptions: {
        dateStyle: "short",
        timeStyle: "medium",
      },
    },
  },
  markdown: {
    theme: {
      light: "github-light",
      dark: "github-dark",
    },
    codeTransformers: [
      {
        preprocess(code, options) {
          if (options.lang === "csharp" || options.lang === "cs") {
            return code;
          }
        },
      },
    ],
  },
});
