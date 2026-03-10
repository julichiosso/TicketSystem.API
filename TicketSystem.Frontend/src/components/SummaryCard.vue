<template>
 <div class="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-xl overflow-hidden hover:border-slate-200 dark:border-slate-700 transition-colors shadow-lg">
 <div class="p-6">
 <div class="flex items-center justify-between mb-2">
 <span class="text-slate-500 text-xs font-bold uppercase tracking-widest">{{ title }}</span>
 <component :is="icon" class="w-5 h-5 text-blue-500" v-if="icon" />
 </div>
 <div class="flex items-baseline gap-2">
 <span class="text-3xl font-bold text-slate-900 dark:text-white tracking-tight">{{ value }}</span>
 <span v-if="trend" :class="trendClass" class="text-xs font-medium">{{ trend }}</span>
 </div>
 </div>
 </div>
</template>
<script setup>
import { computed } from 'vue';
defineProps({
 title: String,
 value: [String, Number],
 icon: Object,
 trend: String,
 trendType: {
 type: String,
 default: 'neutral'
 }
});
const trendClass = computed(() => {
 return {
 'text-emerald-400': trendType === 'up',
 'text-rose-400': trendType === 'down',
 'text-slate-500': trendType === 'neutral'
 };
});
</script>
