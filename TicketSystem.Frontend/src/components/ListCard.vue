<template>
 <div class=\"bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 shadow-sm rounded-2xl p-6 border border-slate-200 dark:border-slate-700\">
 <div class=\"flex items-center justify-between mb-6\">
 <h3 class=\"text-lg font-black text-slate-900 dark:text-white\">{{ title }}</h3>
 <div class=\"text-xs font-bold text-slate-500 uppercase\">{{ items.length }}</div>
 </div>
 <div class=\"space-y-3\">
 <div v-for=\"item in items\" :key=\"item.id\" class=\"flex items-center gap-4 p-3 bg-white dark:bg-slate-900 rounded-lg hover:bg-white dark:bg-slate-900 transition\">
 <div :class=\"`w-1 h-10 rounded-full ${item.color}`\"></div>
 <div class=\"flex-1\">
 <div class=\"text-sm font-semibold text-slate-900 dark:text-white\">{{ item.title }}</div>
 <div class=\"text-xs text-slate-500\">{{ item.subtitle }}</div>
 </div>
 <div v-if=\"item.badge\" :class=\"item.badge.class\" class=\"text-xs font-bold px-2 py-1 rounded\">
 {{ item.badge.text }}
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
