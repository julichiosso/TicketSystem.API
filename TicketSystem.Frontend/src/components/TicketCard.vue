<template>
  <div class="group relative bg-white dark:bg-slate-900 border border-slate-100 dark:border-slate-800/80 shadow-sm rounded-2xl p-4 md:p-6 transition-all duration-300 hover:shadow-md hover:border-blue-100 active:scale-[0.99] hover:translate-y-[-2px]">
    
    <div class="flex items-center justify-between mb-4">
      <div class="flex gap-2">
        <StatusBadge :type="ticket.status" :label="ticket.status" />
        <StatusBadge :type="ticket.priority" :label="ticket.priority" />
      </div>
      <div class="text-[10px] font-medium text-slate-400 bg-slate-50 dark:bg-slate-950 px-2 py-1 rounded-md border border-slate-100 dark:border-slate-800/80">
        #{{ ticket.id?.substring(0, 8) }}
      </div>
    </div>
    
    <h3 class="text-slate-800 dark:text-slate-200 font-bold text-base mb-2 group-hover:text-blue-600 transition-colors line-clamp-1">
      {{ ticket.title }}
    </h3>
    <p class="text-slate-500 text-sm line-clamp-2 mb-6 leading-relaxed">
      {{ ticket.description }}
    </p>
    
    <div class="flex items-center justify-between pt-4 border-t border-slate-50">
      <div class="flex items-center gap-3">
        <div class="w-8 h-8 rounded-full bg-white/5 border border-slate-200 dark:border-slate-700 flex items-center justify-center text-xs font-bold text-primary">
          {{ (ticket.userName || 'U')[0] }}
        </div>
        <div class="text-xs">
          <div class="text-slate-700 font-semibold">{{ ticket.userName || 'User' }}</div>
          <div class="text-slate-500">{{ formatDate(ticket.createdAt) }}</div>
        </div>
      </div>
      
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script setup>
import StatusBadge from './StatusBadge.vue';

defineProps({
  ticket: Object
});

const formatDate = (dateString) => {
  if (!dateString) return '';
  const d = new Date(dateString);
  if (isNaN(d)) return '';
  return d.toLocaleDateString('en-US', {
    day: '2-digit',
    month: 'short'
  });
};
</script>
