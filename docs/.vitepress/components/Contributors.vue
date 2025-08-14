<template>
  <div>
    <div v-if="loading" class="loading">Loading contributors...</div>

    <div v-else-if="error" class="error">
      <p>Unable to load contributors at the moment.</p>
      <p>
        <a
          href="https://github.com/amrohan/termix/graphs/contributors"
          target="_blank"
        >
          View contributors on GitHub →
        </a>
      </p>
    </div>

    <div v-else class="contributors">
      <div
        v-for="contributor in contributors"
        :key="contributor.id"
        class="contributor"
        @click="openProfile(contributor.html_url)"
      >
        <img
          :src="contributor.avatar_url"
          :alt="contributor.login"
          width="60"
          height="60"
        />
        <div class="contributor-info">
          <h4>{{ contributor.name || contributor.login }}</h4>
          <p class="username">@{{ contributor.login }}</p>
          <p class="contributions">
            {{ contributor.contributions }} contributions
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";

const contributors = ref([]);
const loading = ref(true);
const error = ref(false);

const fetchContributors = async () => {
  try {
    const response = await fetch(
      "https://api.github.com/repos/amrohan/termix/contributors?per_page=100",
      {
        headers: {
          Accept: "application/vnd.github.v3+json",
          "User-Agent": "Termix-Documentation-Site",
        },
      },
    );

    if (!response.ok) {
      if (response.status === 403) {
        throw new Error("API rate limit exceeded. Please try again later.");
      } else if (response.status === 404) {
        throw new Error("Repository not found.");
      } else {
        throw new Error(`GitHub API error: ${response.status}`);
      }
    }

    const data = await response.json();

    const sortedContributors = data.sort(
      (a, b) => b.contributions - a.contributions,
    );

    contributors.value = sortedContributors;
    loading.value = false;
  } catch (err) {
    error.value = true;
    loading.value = false;
  }
};

const openProfile = (url) => {
  window.open(url, "_blank");
};

onMounted(() => {
  fetchContributors();
});
</script>

<style scoped>
.contributors {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
  margin: 20px 0;
}

.contributor {
  display: flex;
  align-items: center;
  padding: 16px;
  border: 1px solid var(--vp-c-border);
  border-radius: 12px;
  background: var(--vp-c-bg-soft);
  transition: all 0.3s ease;
  cursor: pointer;
}

.contributor:hover {
  border-color: var(--vp-c-brand-1);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.contributor img {
  flex-shrink: 0;
  margin-right: 16px;
  border-radius: 50%;
  border: 2px solid var(--vp-c-border);
  transition: border-color 0.3s ease;
}

.contributor:hover img {
  border-color: var(--vp-c-brand-1);
}

.contributor-info {
  flex-grow: 1;
}

.contributor h4 {
  color: var(--vp-c-text-1);
  font-weight: 600;
  margin: 0 0 4px 0;
  font-size: 16px;
}

.contributor .username {
  color: var(--vp-c-text-2);
  font-size: 14px;
  margin: 0 0 6px 0;
}

.contributor .contributions {
  color: var(--vp-c-brand-1);
  font-size: 12px;
  font-weight: 500;
  margin: 0;
}

.loading {
  text-align: center;
  padding: 40px;
  color: var(--vp-c-text-2);
  font-style: italic;
}

.error {
  text-align: center;
  padding: 40px;
  color: var(--vp-c-text-2);
  background: var(--vp-c-bg-soft);
  border: 1px solid var(--vp-c-border);
  border-radius: 8px;
}

.error a {
  color: var(--vp-c-brand-1);
  text-decoration: none;
}

.error a:hover {
  text-decoration: underline;
}

@media (max-width: 768px) {
  .contributors {
    grid-template-columns: 1fr;
  }
}
</style>
