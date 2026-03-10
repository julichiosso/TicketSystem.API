<template>
 <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
 <div v-for="item in items" :key="item.label" class="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 shadow-sm rounded-2xl p-6">
 <h4 class="text-xs font-black uppercase tracking-widest text-slate-500 mb-2">{{ item.label }}</h4>
 <div class="text-2xl font-black text-slate-900 dark:text-white mb-2">{{ item.value }}</div>
 <div class="text-xs text-slate-500 flex items-center gap-2">
 <div :class="`w-2 h-2 rounded-full ${item.status === 'good' ? 'bg-emerald-500' : item.status === 'warning' ? 'bg-amber-500' : 'bg-rose-500'}`"></div>
 {{ item.description }}
 </div>
 </div>
 </div>
</template>
<script setup>
defineProps({
 items: Array
});
</script>
