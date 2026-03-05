<template>
  <div :class="settingsStore.isDark ? 'dark' : ''" class="min-h-screen transition-colors duration-300">
    <router-view />
    <NotificationToast />
  </div>
</template>

<script setup>
import { onMounted, watch } from 'vue';
import NotificationToast from './components/NotificationToast.vue';
import { useSettingsStore } from './store/settings';

const settingsStore = useSettingsStore();

onMounted(() => {
  settingsStore.initSettings();
});

// Reactivo: cuando cambia el tema se aplica al html
watch(() => settingsStore.themeMode, () => {
  settingsStore.applyTheme();
});
</script>