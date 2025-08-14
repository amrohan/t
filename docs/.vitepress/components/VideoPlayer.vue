<template>
  <div class="video-container">
    <video
      :src="src"
      autoplay
      muted
      loop
      playsinline
      preload="metadata"
      class="video-player"
    ></video>
  </div>
</template>

<script setup>
defineProps({
  src: {
    type: String,
    required: true,
  },
});
</script>

<style scoped>
.video-container {
  display: flex;
  justify-content: center;
  margin: 1rem 0;
}

.video-player {
  max-width: 100%;
  height: auto;
  border-radius: 8px;
}
</style>
