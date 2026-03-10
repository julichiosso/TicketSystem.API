<template>
 <div class="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 shadow-sm rounded-2xl p-6 border border-slate-200 dark:border-slate-700">
 <div class="flex items-center justify-between mb-6">
 <h3 class="text-lg font-black text-slate-900 dark:text-white">{{ title }}</h3>
 <div class="text-xs font-bold text-slate-500 uppercase">{{ items.length }} total</div>
 </div>
 <div class="space-y-2">
 <div v-for="item in items" :key="item.id" class="flex items-center justify-between p-3 bg-white dark:bg-slate-900 rounded-lg hover:bg-white dark:bg-slate-900 transition">
 <div>
 <div class="text-sm font-semibold text-slate-900 dark:text-white">{{ item.label }}</div>
 <div class="text-xs text-slate-500">{{ item.description }}</div>
 </div>
 <div class="text-right">
 <div class="text-xl font-bold text-blue-400">{{ item.value }}</div>
 <div class="text-[10px] text-slate-500">{{ item.change }}</div>
 </div>
 </div>
 </div>
 </div>
</template>
<script setup>
defineProps({
 title: String,
 items: Array
});
</script>
