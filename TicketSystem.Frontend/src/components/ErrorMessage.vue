<template>
 <Transition name="fade-slide">
 <div v-if="message" class="bg-rose-500/10 border border-rose-500/20 text-rose-400 text-sm px-4 py-3 rounded-xl flex items-center gap-3 font-medium">
 <AlertCircleIcon class="w-4 h-4 flex-shrink-0" />
 <span>{{ message }}</span>
 </div>
 </Transition>
</template>
<script setup>
import { AlertCircleIcon } from 'lucide-vue-next';
defineProps({
 message: String
});
</script>
<style scoped>
.fade-slide-enter-active,
.fade-slide-leave-active {
 transition: all 0.3s ease;
}
.fade-slide-enter-from {
 opacity: 0;
 transform: translateY(-8px);
}
.fade-slide-leave-to {
 opacity: 0;
 transform: translateY(-8px);
}
</style>
