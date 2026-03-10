<template>
  <div v-if="ticketsStore.totalPages > 1"
    class="flex items-center justify-between px-2 py-4">
    <!-- Info -->
    <p class="text-xs font-medium"
      :class="'text-slate-400 dark:text-slate-500'">
      Mostrando
      <span class="font-black"
        :class="'text-slate-900 dark:text-white'">
        {{ desde }}–{{ hasta }}
      </span>
      de
      <span class="font-black"
        :class="'text-slate-900 dark:text-white'">
        {{ ticketsStore.totalTickets }}
      </span>
      tickets
    </p>
    <!-- Controles -->
    <div class="flex items-center gap-1">
      <!-- Anterior -->
      <button @click="ticketsStore.goToPage(ticketsStore.currentPage - 1)"
        :disabled="ticketsStore.currentPage === 1"
        class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
        :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-500 hover:border-slate-300 dark:bg-slate-800 dark:border-slate-700 dark:text-slate-400 dark:hover:border-slate-600'">
        <ChevronLeftIcon class="w-4 h-4" />
      </button>
      <!-- Páginas -->
      <template v-for="page in visiblePages" :key="page">
        <span v-if="page === '...'"
          class="px-2 text-xs"
          :class="'text-slate-400 dark:text-slate-600'">
          ···
        </span>
        <button v-else
          @click="ticketsStore.goToPage(page)"
          class="w-8 h-8 rounded-lg border text-xs font-black transition-all"
          :class="page === ticketsStore.currentPage
            ? 'bg-blue-600 border-blue-600 text-white shadow-sm'
            : settingsStore.isDark
              ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600'
              : 'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-600 hover:border-slate-300'">
          {{ page }}
        </button>
      </template>
      <!-- Siguiente -->
      <button @click="ticketsStore.goToPage(ticketsStore.currentPage + 1)"
        :disabled="ticketsStore.currentPage === ticketsStore.totalPages"
        class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
        :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-500 hover:border-slate-300 dark:bg-slate-800 dark:border-slate-700 dark:text-slate-400 dark:hover:border-slate-600'">
        <ChevronRightIcon class="w-4 h-4" />
      </button>
    </div>
    <!-- PageSize -->
    <div class="flex items-center gap-2">
      <span class="text-xs"
        :class="'text-slate-400 dark:text-slate-500'">
        Por página
      </span>
      <select @change="ticketsStore.changePageSize(parseInt($event.target.value))"
        :value="ticketsStore.pageSize"
        class="border px-2 py-1 rounded-lg text-xs outline-none transition"
        :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-700 dark:bg-slate-800 dark:border-slate-700 dark:text-white'">
        <option value="10">10</option>
        <option value="25">25</option>
        <option value="50">50</option>
      </select>
    </div>
  </div>
</template>
<script setup>
import { computed } from 'vue';
import { ChevronLeftIcon, ChevronRightIcon } from 'lucide-vue-next';
import { useTicketsStore } from '../store/tickets';
import { useSettingsStore } from '../store/settings';
const ticketsStore  = useTicketsStore();
const settingsStore = useSettingsStore();
const desde = computed(() =>
  (ticketsStore.currentPage - 1) * ticketsStore.pageSize + 1
);
const hasta = computed(() =>
  Math.min(ticketsStore.currentPage * ticketsStore.pageSize, ticketsStore.totalTickets)
);
const visiblePages = computed(() => {
  const total   = ticketsStore.totalPages;
  const current = ticketsStore.currentPage;
  const pages   = [];
  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i);
    return pages;
  }
  pages.push(1);
  if (current > 3)          pages.push('...');
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++)
    pages.push(i);
  if (current < total - 2)  pages.push('...');
  pages.push(total);
  return pages;
});
const currentPage = ref(1);
const PAGE_SIZE   = 12;
const allAdminTickets = computed(() => {
  if (!ticketsSearch.value) return ticketsStore.tickets;
  return ticketsStore.tickets.filter(t =>
    t.titulo?.toLowerCase().includes(ticketsSearch.value.toLowerCase()) ||
    t.usuarioNombre?.toLowerCase().includes(ticketsSearch.value.toLowerCase())
  );
});
const totalPages = computed(() => Math.ceil(allAdminTickets.value.length / PAGE_SIZE) || 1);
const pagedTickets = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE;
  return allAdminTickets.value.slice(start, start + PAGE_SIZE);
});
const visiblePages = computed(() => {
  const total = totalPages.value, current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages = [1];
  if (current > 3) pages.push('...');
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) pages.push(i);
  if (current < total - 2) pages.push('...');
  pages.push(total);
  return pages;
});
const goToPage = (p) => {
  if (p < 1 || p > totalPages.value) return;
  currentPage.value = p;
};
</script>