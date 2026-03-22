<template>
  <div class="min-h-screen flex transition-colors duration-300"
    :class="'bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-white'">
    <Sidebar />
    <main class="flex-1 p-4 md:p-8 pt-20 md:pt-8 space-y-6 overflow-y-auto h-screen custom-scrollbar">
      <header class="flex flex-col gap-6 md:flex-row md:items-center justify-between">
        <div>
          <h2 class="text-xl font-semibold tracking-tight">Support Dashboard</h2>
          <p class="text-[10px] font-medium uppercase tracking-[0.1em]"
            :class="'text-slate-400 dark:text-slate-500'">
            Resolution Operator
          </p>
        </div>
        <div class="flex flex-wrap items-center gap-4">
          <div class="p-1 rounded-xl border flex gap-1"
            :class="'bg-slate-100 border-slate-200 dark:bg-slate-900 dark:border-slate-800'">
            <button v-for="tab in tabs" :key="tab.key" @click="activeTab = tab.key"
              class="px-5 py-2 rounded-lg text-[10px] font-bold uppercase tracking-widest transition-all duration-200"
              :class="activeTab === tab.key
                ? settingsStore.isDark ? 'bg-slate-800 text-blue-400 shadow-sm' : 'bg-white text-blue-600 shadow-sm'
                : settingsStore.isDark ? 'text-slate-500 hover:text-slate-300' : 'text-slate-400 hover:text-slate-600'">
              {{ tab.label }}
            </button>
          </div>
          <button @click="fetchTickets"
            class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
            :class="'hover:bg-slate-200 text-slate-500 dark:hover:bg-slate-800 dark:text-slate-400'">
            <RefreshCcwIcon class="w-5 h-5" />
          </button>
        </div>
      </header>

      <section v-if="activeTab === 'tickets'" class="space-y-4">
        <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
          <div v-for="metric in metrics" :key="metric.label"
            class="rounded-xl p-4 border transition-all hover:scale-[1.02]"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <div class="text-xs mb-1" :class="'text-slate-400 dark:text-slate-500'">{{ metric.label }}</div>
            <div class="text-2xl font-bold">{{ metric.value }}</div>
          </div>
        </div>

        <div v-if="isLoadingTickets" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 6" :key="n" />
        </div>
        <div v-else-if="pagedTickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in pagedTickets" :key="ticket.id"
            :ticket="ticket" @click="openDetail(ticket)" class="cursor-pointer">
            <template #actions>
              <div class="flex items-center gap-2">
                <button @click.stop="updateStatus(ticket.id, 1)"
                  class="px-2 py-1 rounded bg-indigo-500/20 text-indigo-400 text-xs font-medium hover:bg-indigo-500/30 transition-colors">
                  Process
                </button>
                <button @click.stop="updateStatus(ticket.id, 4)"
                  class="px-2 py-1 rounded bg-amber-100 dark:bg-amber-900/40 text-amber-700 dark:text-amber-400 text-xs font-medium hover:bg-amber-500/30 transition-colors">
                  Wait
                </button>
                <button @click.stop="updateStatus(ticket.id, 2)"
                  class="px-2 py-1 rounded bg-emerald-500/20 text-emerald-400 text-xs font-medium hover:bg-emerald-500/30 transition-colors">
                  Resolve
                </button>
              </div>
            </template>
          </TicketCard>
        </div>
        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] transition-colors"
          :class="'border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900/50'">
          <div class="w-24 h-24 rounded-full flex items-center justify-center mb-8 border transition-colors"
            :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700'">
            <InboxIcon class="w-10 h-10" :class="'text-slate-300 dark:text-slate-600'" />
          </div>
          <h3 class="text-xl font-black mb-2 uppercase italic tracking-tight">No assigned tickets</h3>
          <p class="text-sm max-w-sm text-center font-medium leading-relaxed"
            :class="'text-slate-400 dark:text-slate-500'">
            You have no assigned tickets yet. The administrator will assign them soon.
          </p>
        </div>

        <div v-if="totalPages > 1"
          class="flex items-center justify-between pt-4 border-t"
          :class="'border-slate-100 dark:border-slate-800'">
          <p class="text-xs font-medium" :class="'text-slate-400 dark:text-slate-500'">
            Showing
            <span class="font-black" :class="'text-slate-900 dark:text-white'">
              {{ (currentPage - 1) * PAGE_SIZE + 1 }}–{{ Math.min(currentPage * PAGE_SIZE, assignedTickets.length) }}
            </span>
            of
            <span class="font-black" :class="'text-slate-900 dark:text-white'">{{ assignedTickets.length }}</span>
            tickets
          </p>
          <div class="flex items-center gap-1">
            <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 dark:text-slate-400 hover:border-slate-300 dark:hover:border-slate-600'">
              <ChevronLeftIcon class="w-4 h-4" />
            </button>
            <template v-for="p in visiblePages" :key="p">
              <span v-if="p === '...'" class="px-2 text-xs" :class="'text-slate-400 dark:text-slate-600'">···</span>
              <button v-else @click="goToPage(p)"
                class="w-8 h-8 rounded-lg border text-xs font-black transition-all"
                :class="p === currentPage
                  ? 'bg-blue-600 border-blue-600 text-white shadow-sm'
                  : settingsStore.isDark
                    ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600'
                    : 'bg-white border-slate-200 text-slate-600 hover:border-slate-300'">
                {{ p }}
              </button>
            </template>
            <button @click="goToPage(currentPage + 1)" :disabled="currentPage === totalPages"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 dark:text-slate-400 hover:border-slate-300 dark:hover:border-slate-600'">
              <ChevronRightIcon class="w-4 h-4" />
            </button>
          </div>
          <p class="text-[10px] font-bold uppercase tracking-widest" :class="'text-slate-400 dark:text-slate-600'">
            Page {{ currentPage }} of {{ totalPages }}
          </p>
        </div>
      </section>

      <section v-else-if="activeTab === 'stats'" class="space-y-6">
        <div class="grid md:grid-cols-2 gap-6">
          <div class="rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <h3 class="text-lg font-black mb-6">Status Distribution</h3>
            <div class="relative h-64 flex items-center justify-center">
              <canvas ref="donutChartRef"></canvas>
            </div>
          </div>
          <div class="rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <h3 class="text-lg font-black mb-6">My Statistics</h3>
            <div class="space-y-3">
              <div v-for="stat in myStats" :key="stat.label"
                class="flex items-center justify-between p-3 rounded-lg"
                :class="'bg-slate-50 dark:bg-slate-800'">
                <span class="text-sm" :class="'text-slate-500 dark:text-slate-400'">{{ stat.label }}</span>
                <span class="font-bold" :class="stat.color">{{ stat.value }}</span>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <TicketDetailModal v-if="ticketsStore.selectedTicket"
      :ticket="ticketsStore.selectedTicket"
      @close="ticketsStore.clearSelection()" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useAuthStore, API_URL } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import { useSettingsStore } from '../store/settings';
import { useNotificationStore } from '../store/notifications';
import { RefreshCcwIcon, InboxIcon, ChevronLeftIcon, ChevronRightIcon } from 'lucide-vue-next';
import TicketDetailModal  from '../components/TicketDetailModal.vue';
import TicketCard         from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import Sidebar            from '../components/Sidebar.vue';

const router            = useRouter();
const authStore         = useAuthStore();
const ticketsStore      = useTicketsStore();
const settingsStore     = useSettingsStore();
const notificationStore = useNotificationStore();

const activeTab = ref('tickets');
const tabs = [
  { key: 'tickets', label: 'My Tickets' },
  { key: 'stats',   label: 'Statistics' },
];

const assignedTickets  = ref([]);
const isLoadingTickets = ref(false);
const currentPage      = ref(1);
const PAGE_SIZE        = 12;

const donutChartRef = ref(null);
let donutChart      = null;

const metrics = computed(() => {
  const total      = assignedTickets.value.length;
  const inProgress = assignedTickets.value.filter(t => t.status === 1 || t.status === 'InProgress').length;
  const resolved   = assignedTickets.value.filter(t => t.status === 2 || t.status === 'Resolved').length;
  const waiting    = assignedTickets.value.filter(t => t.status === 4 || t.status === 'WaitingForUser').length;
  return [
    { label: 'Total',       value: total },
    { label: 'In Progress', value: inProgress },
    { label: 'Waiting',     value: waiting },
    { label: 'Resolved',    value: resolved },
  ];
});

const resolutionRate = computed(() => {
  const total    = metrics.value[0].value;
  const resolved = metrics.value[3].value;
  return total > 0 ? Math.round((resolved / total) * 100) : 0;
});

const myStats = computed(() => [
  { label: 'Total Tickets',    value: metrics.value[0].value, color: 'text-blue-400' },
  { label: 'In Progress',      value: metrics.value[1].value, color: 'text-indigo-400' },
  { label: 'Resolved',         value: metrics.value[3].value, color: 'text-emerald-400' },
  { label: 'Resolution Rate',  value: resolutionRate.value + '%', color: 'text-purple-400' },
]);

const totalPages = computed(() => Math.ceil(assignedTickets.value.length / PAGE_SIZE) || 1);
const pagedTickets = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE;
  return assignedTickets.value.slice(start, start + PAGE_SIZE);
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

const isDark = computed(() => settingsStore.isDark);

const buildDonutChart = async () => {
  await nextTick();
  if (!donutChartRef.value) return;

  const { Chart, ArcElement, DoughnutController, Tooltip, Legend } = await import('chart.js');
  Chart.register(ArcElement, DoughnutController, Tooltip, Legend);

  const textColor = isDark.value ? '#94a3b8' : '#64748b';
  const data = [
    metrics.value[1].value,
    metrics.value[2].value,
    metrics.value[3].value,
    assignedTickets.value.filter(t => t.status === 'Pending' || t.status === 0).length
  ];

  if (donutChart) donutChart.destroy();
  donutChart = new Chart(donutChartRef.value, {
    type: 'doughnut',
    data: {
      labels: ['In Progress', 'Waiting', 'Resolved', 'Pending'],
      datasets: [{
        data,
        backgroundColor: ['#6366f1','#f59e0b','#10b981','#3b82f6'],
        borderWidth: 0,
        hoverOffset: 6
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '65%',
      plugins: {
        legend: {
          position: 'bottom',
          labels: { color: textColor, padding: 12, font: { size: 11, weight: 'bold' } }
        }
      }
    }
  });
};

const fetchTickets = async () => {
  isLoadingTickets.value = true;
  try {
    const res = await axios.get(`${API_URL}/tickets/operator/my-tickets`, {
      params: { page: 1, pageSize: 200 }
    });
    const result = res.data?.data;
    assignedTickets.value = Array.isArray(result?.data) ? result.data : Array.isArray(result) ? result : [];
    currentPage.value = 1;
  } catch (e) {
    console.error('Error fetching operator tickets:', e);
  } finally {
    isLoadingTickets.value = false;
  }
};

const updateStatus = async (ticketId, newStatus) => {
  try {
    await axios.patch(
      `${API_URL}/tickets/${ticketId}/status`,
      { status: newStatus },
      { headers: { 'Content-Type': 'application/json' } }
    );
    const statusMap = { 1: 'InProgress', 2: 'Resolved', 4: 'WaitingForUser' };
    const newState = statusMap[newStatus] ?? newStatus;
    const index = assignedTickets.value.findIndex(t => t.id === ticketId);
    if (index !== -1) assignedTickets.value[index] = { ...assignedTickets.value[index], status: newState };
    notificationStore.success('Status updated.');
  } catch (e) {
    console.error('Error updating status:', e);
    notificationStore.error('Could not change status.');
  }
};

const openDetail = async (ticket) => {
  ticketsStore.selectTicket(ticket);
  await ticketsStore.fetchComments(ticket.id);
};

onMounted(() => {
  if (!authStore.isOperator) { router.push('/'); return; }
  fetchTickets();
});

watch(activeTab, (v) => {
  if (v === 'stats') nextTick(buildDonutChart);
});

watch(isDark, () => {
  if (activeTab.value === 'stats') nextTick(buildDonutChart);
});
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 5px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.1); border-radius: 10px; }
</style>
