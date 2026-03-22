<template>
  <div class="min-h-screen flex font-sans select-none transition-colors duration-300"
    :class="'bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-white'">
    <Sidebar />
    <main class="flex-1 min-w-0 h-screen overflow-y-auto page-fade-in custom-scrollbar p-4 md:p-0">
      <header class="h-20 flex items-center justify-between px-10 sticky top-0 z-30 border-b transition-colors"
        :class="'bg-white dark:bg-slate-900 border-slate-100 dark:border-slate-800'">
        <div>
          <h2 class="text-xl font-semibold tracking-tight">My Tickets</h2>
          <p class="text-[10px] font-bold uppercase tracking-[0.2em]"
            :class="'text-slate-400 dark:text-slate-500'">
            Support & Tracking
          </p>
        </div>
        <div class="flex items-center gap-4">
          <button @click="reload"
            class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
            :class="'hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-500 dark:text-slate-400'">
            <RefreshCcwIcon class="w-5 h-5" />
          </button>
          <div class="h-8 w-px" :class="'bg-slate-200 dark:bg-slate-700'"></div>
          <div class="relative profile-menu-container">
            <button @click.stop="showProfileMenu = !showProfileMenu"
              class="flex items-center gap-3 group cursor-pointer">
              <div class="text-right hidden sm:block">
                <div class="text-xs font-black uppercase tracking-wider group-hover:text-blue-400 transition-colors">
                  {{ authStore.user?.name }}
                </div>
                <div class="text-[9px] font-bold uppercase tracking-widest"
                  :class="'text-slate-400 dark:text-slate-500'">
                  {{ authStore.user?.role }}
                </div>
              </div>
              <div :class="`w-9 h-9 rounded-xl bg-gradient-to-br ${settingsStore.themeClasses} flex items-center justify-center font-black text-white text-sm shadow-sm`">
                {{ authStore.user?.name?.[0] }}
              </div>
            </button>
            <div v-if="showProfileMenu"
              class="absolute right-0 mt-3 w-52 rounded-2xl shadow-lg py-2 z-50 border"
              :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800'">
              <router-link to="/profile"
                class="flex items-center gap-3 px-5 py-2.5 transition-all text-sm font-medium"
                :class="'text-slate-500 hover:text-slate-900 hover:bg-slate-50 dark:text-slate-400 dark:hover:text-white dark:hover:bg-slate-800'">
                <UserIcon class="w-4 h-4" /> My Profile
              </router-link>
              <router-link to="/settings"
                class="flex items-center gap-3 px-5 py-2.5 transition-all text-sm font-medium"
                :class="'text-slate-500 hover:text-slate-900 hover:bg-slate-50 dark:text-slate-400 dark:hover:text-white dark:hover:bg-slate-800'">
                <SettingsIcon class="w-4 h-4" /> Settings
              </router-link>
              <div class="h-px my-1 mx-4" :class="'bg-slate-100 dark:bg-slate-800'"></div>
              <button @click="handleLogout"
                class="w-full flex items-center gap-3 px-5 py-2.5 text-rose-500 hover:bg-rose-500/10 transition-all text-sm font-medium">
                <LogOutIcon class="w-4 h-4" /> Log Out
              </button>
            </div>
          </div>
        </div>
      </header>

      <div class="max-w-[1600px] mx-auto p-4 md:p-10 pt-20 md:pt-10">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <div class="md:col-span-3 rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-100 dark:border-slate-800 shadow-sm'">
            <div class="flex items-center justify-between">
              <div>
                <h2 class="text-xl font-bold mb-1">Hello, {{ authStore.user?.name }}!</h2>
                <p class="text-sm" :class="'text-slate-500 dark:text-slate-400'">
                  Manage and track your support tickets
                </p>
              </div>
              <div class="text-5xl opacity-10">🎟️</div>
            </div>
          </div>
          <div class="rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-100 dark:border-slate-800 shadow-sm'">
            <p class="text-[10px] font-semibold uppercase tracking-wider mb-1"
              :class="'text-slate-400 dark:text-slate-500'">Avg Response Time</p>
            <h3 class="text-2xl font-bold text-blue-500">~2h</h3>
            <p class="text-xs mt-1" :class="'text-slate-400 dark:text-slate-500'">Response</p>
          </div>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
          <div v-for="stat in statCards" :key="stat.label"
            class="rounded-2xl p-6 border transition-all hover:scale-[1.01]"
            :class="'bg-white dark:bg-slate-900 border-slate-100 dark:border-slate-800 shadow-sm'">
            <p class="text-[9px] font-bold uppercase tracking-widest mb-1"
              :class="'text-slate-400 dark:text-slate-500'">{{ stat.label }}</p>
            <h4 class="text-3xl font-bold mb-1">{{ stat.value }}</h4>
            <p class="text-xs" :class="'text-slate-400 dark:text-slate-500'">{{ stat.desc }}</p>
          </div>
        </div>

        <div class="flex flex-col sm:flex-row items-center justify-between mb-6 gap-4">
          <div class="flex items-center gap-4">
            <h3 class="text-sm font-black flex items-center gap-3 uppercase tracking-widest">
              <div :class="`w-1 h-5 bg-gradient-to-b ${settingsStore.themeClasses} rounded-full`"></div>
              My Tickets
            </h3>
            <button @click="showModal = true"
              class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-colors shadow-sm">
              <PlusIcon class="w-4 h-4 transition-transform group-hover:rotate-90 duration-300" />
              New Ticket
            </button>
          </div>
          <div class="flex items-center gap-3">
            <div class="relative">
              <input v-model="searchQuery" type="text" placeholder="Search..."
                class="border pl-9 pr-4 py-2 rounded-xl text-sm outline-none transition w-56"
                :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 placeholder-slate-400 focus:border-blue-500 dark:text-white dark:placeholder-slate-600'" />
              <SearchIcon class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2"
                :class="'text-slate-400 dark:text-slate-600'" />
            </div>
            <div class="flex gap-1">
              <button v-for="f in fastFilters" :key="f.key"
                @click="toggleFastFilter(f.key)"
                class="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border transition-all"
                :class="activeFilter === f.key
                  ? 'bg-blue-600 border-blue-600 text-white'
                  : 'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-400 hover:border-slate-300'">
                {{ f.label }}
              </button>
            </div>
            <div class="text-[10px] font-bold uppercase tracking-widest px-3 py-1.5 rounded-lg border"
              :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-slate-400 dark:text-slate-500'">
              {{ totalTickets }} tickets
            </div>
          </div>
        </div>

        <div v-if="isLoading" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 8" :key="n" />
        </div>

        <div v-else-if="filteredTickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in filteredTickets" :key="ticket.id"
            :ticket="ticket" @click="openTicketDetail(ticket)" class="cursor-pointer" />
        </div>

        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] transition-colors"
          :class="'border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900/50'">
          <InboxIcon class="w-10 h-10 mb-4" :class="'text-slate-300 dark:text-slate-700'" />
          <h3 class="text-xl font-black mb-2 uppercase italic">
            {{ totalTickets === 0 ? 'Your inbox is empty' : 'No results' }}
          </h3>
          <p class="text-sm max-w-sm text-center font-medium"
            :class="'text-slate-400 dark:text-slate-500'">
            {{ totalTickets === 0
              ? 'You have no active tickets. Create one if you need support.'
              : `No results for "${searchQuery}".` }}
          </p>
          <button v-if="totalTickets === 0" @click="showModal = true"
            class="mt-6 flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-medium px-6 py-3 rounded-lg transition-colors">
            <PlusIcon class="w-5 h-5" /> Create my first ticket
          </button>
        </div>

        <div v-if="totalPages > 1"
          class="flex items-center justify-between mt-8 pt-6 border-t"
          :class="'border-slate-100 dark:border-slate-800'">
          <p class="text-xs font-medium" :class="'text-slate-400 dark:text-slate-500'">
            Page
            <span class="font-black" :class="'text-slate-900 dark:text-white'">{{ currentPage }}</span>
            of
            <span class="font-black" :class="'text-slate-900 dark:text-white'">{{ totalPages }}</span>
          </p>
          <div class="flex items-center gap-1">
            <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 hover:border-slate-300'">
              <ChevronLeftIcon class="w-4 h-4" />
            </button>
            <template v-for="p in visiblePages" :key="p">
              <span v-if="p === '...'" class="px-2 text-xs" :class="'text-slate-400 dark:text-slate-600'">···</span>
              <button v-else @click="goToPage(p)"
                class="w-8 h-8 rounded-lg border text-xs font-black transition-all"
                :class="p === currentPage
                  ? 'bg-blue-600 border-blue-600 text-white shadow-sm'
                  : 'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 hover:border-slate-300'">
                {{ p }}
              </button>
            </template>
            <button @click="goToPage(currentPage + 1)" :disabled="currentPage === totalPages"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 hover:border-slate-300'">
              <ChevronRightIcon class="w-4 h-4" />
            </button>
          </div>
          <p class="text-[10px] font-bold uppercase tracking-widest"
            :class="'text-slate-400 dark:text-slate-600'">
            {{ totalTickets }} total
          </p>
        </div>
      </div>
    </main>

    <div v-if="showModal"
      class="fixed inset-0 z-[100] flex items-center justify-center p-6 backdrop-blur-sm"
      :class="'bg-slate-900/30 dark:bg-slate-950/70'">
      <div class="rounded-[2.5rem] w-full max-w-xl shadow-xl border"
        :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800'">
        <div class="p-10">
          <div class="flex justify-between items-start mb-8">
            <div>
              <h3 class="text-2xl font-bold">New Ticket</h3>
              <p class="text-sm mt-1" :class="'text-slate-500 dark:text-slate-400'">
                Describe your problem in detail
              </p>
            </div>
            <button @click="showModal = false"
              class="p-2 rounded-full transition-all"
              :class="'hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-400'">
              <XIcon class="w-5 h-5" />
            </button>
          </div>
          <form @submit.prevent="createTicket" class="space-y-5">
            <div class="relative">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="'bg-white dark:bg-slate-900 text-slate-400 dark:text-slate-500'">
                Subject
              </label>
              <input v-model="newTicket.title" maxlength="100"
                class="w-full border-2 px-5 py-3.5 rounded-2xl outline-none transition font-medium text-sm"
                :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white focus:border-blue-500'"
                placeholder="E.g.: Payment processing error" />
              <p v-if="formErrors.title" class="text-rose-500 text-xs mt-1 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.title }}
              </p>
            </div>
            <div class="relative">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="'bg-white dark:bg-slate-900 text-slate-400 dark:text-slate-500'">
                Description
              </label>
              <textarea v-model="newTicket.description" rows="4" maxlength="1000"
                class="w-full border-2 px-5 py-3.5 rounded-2xl outline-none transition font-medium text-sm resize-none"
                :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white focus:border-blue-500'"
                placeholder="What's going wrong exactly?"></textarea>
              <p v-if="formErrors.description" class="text-rose-500 text-xs mt-1 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.description }}
              </p>
            </div>
            <div class="flex gap-3">
              <button v-for="(opt, key) in priorityOptions" :key="key" type="button"
                @click="newTicket.priority = key"
                class="flex-1 py-3 rounded-xl border-2 flex flex-col items-center gap-1 transition-all text-xs font-black uppercase tracking-widest"
                :class="newTicket.priority === key
                  ? 'border-blue-500 bg-blue-500/10 text-blue-500'
                  : 'border-slate-200 dark:border-slate-700 text-slate-400 hover:border-slate-300'">
                <component :is="opt.icon" class="w-4 h-4" :class="opt.colorClass" />
                {{ opt.label }}
              </button>
            </div>
            <div class="flex gap-3 pt-2 border-t" :class="'border-slate-100 dark:border-slate-800'">
              <button @click="showModal = false" type="button"
                class="px-5 py-2.5 border font-medium rounded-lg text-sm transition-colors"
                :class="'bg-white dark:bg-slate-800 border-slate-300 dark:border-slate-700 text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700'">
                Cancel
              </button>
              <button type="submit" :disabled="creatingTicket"
                class="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg shadow-sm transition-colors disabled:opacity-50">
                {{ creatingTicket ? 'Sending...' : 'Submit Request' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <TicketDetailModal v-if="selectedTicket" :ticket="selectedTicket"
      @close="selectedTicket = null; ticketsStore.clearSelection()" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import {
  InboxIcon, RefreshCcwIcon, XIcon, PlusIcon,
  UserIcon, SettingsIcon, SearchIcon, LogOutIcon,
  CheckCircle2Icon, AlertTriangleIcon, AlertOctagonIcon, AlertCircleIcon,
  ChevronLeftIcon, ChevronRightIcon
} from 'lucide-vue-next';
import { useAuthStore, API_URL } from '../store/auth';
import { useNotificationStore } from '../store/notifications';
import { useSettingsStore } from '../store/settings';
import { useTicketsStore } from '../store/tickets';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import Sidebar from '../components/Sidebar.vue';

const authStore         = useAuthStore();
const notificationStore = useNotificationStore();
const settingsStore     = useSettingsStore();
const ticketsStore      = useTicketsStore();
const router            = useRouter();

const allTickets      = ref([]);
const totalTickets    = ref(0);
const totalPages      = ref(1);
const currentPage     = ref(1);
const PAGE_SIZE       = 12;
const isLoading       = ref(false);
const searchQuery     = ref('');
const showModal       = ref(false);
const showProfileMenu = ref(false);
const selectedTicket  = ref(null);
const activeFilter    = ref(null);

const stats = reactive({ open: 0, processing: 0, resolved: 0 });

const statCards = computed(() => [
  { label: 'Pending',   value: stats.open,       desc: 'Waiting' },
  { label: 'In Progress', value: stats.processing, desc: 'Being handled' },
  { label: 'Resolved',  value: stats.resolved,   desc: 'Completed' },
]);

const fastFilters = [
  { key: null,        label: 'All' },
  { key: 'Pending',   label: 'Pending' },
  { key: 'InProgress', label: 'In Progress' },
  { key: 'Resolved',  label: 'Resolved' },
];

const toggleFastFilter = (key) => {
  activeFilter.value = activeFilter.value === key ? null : key;
};

const filteredTickets = computed(() => {
  let result = allTickets.value;
  if (activeFilter.value)
    result = result.filter(t => t.status === activeFilter.value);
  if (searchQuery.value)
    result = result.filter(t =>
      t.title?.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  return result;
});

const visiblePages = computed(() => {
  const total   = totalPages.value;
  const current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages = [1];
  if (current > 3) pages.push('...');
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++)
    pages.push(i);
  if (current < total - 2) pages.push('...');
  pages.push(total);
  return pages;
});

const newTicket      = reactive({ title: '', description: '', priority: '1' });
const creatingTicket = ref(false);
const formErrors     = ref({ title: '', description: '' });

const priorityOptions = {
  '0': { label: 'Low',      icon: CheckCircle2Icon,  colorClass: 'text-emerald-500' },
  '1': { label: 'Medium',   icon: AlertTriangleIcon, colorClass: 'text-amber-500' },
  '2': { label: 'Urgent',   icon: AlertOctagonIcon,  colorClass: 'text-rose-500' },
};

const calcStats = (tickets) => {
  stats.open       = tickets.filter(t => t.status === 'Pending' || t.status === 0).length;
  stats.processing = tickets.filter(t => t.status === 'InProgress' || t.status === 1).length;
  stats.resolved   = tickets.filter(t => t.status === 'Resolved'  || t.status === 2).length;
};

const fetchPage = async (page = 1) => {
  isLoading.value = true;
  try {
    const response = await axios.get(`${API_URL}/tickets/my-tickets`, {
      params: { page, pageSize: PAGE_SIZE }
    });
    const result = response.data?.data;
    allTickets.value  = Array.isArray(result?.data) ? result.data : (Array.isArray(result) ? result : []);
    totalTickets.value = result?.totalRecords ?? allTickets.value.length;
    totalPages.value   = Math.ceil(totalTickets.value / PAGE_SIZE) || 1;
    currentPage.value  = page;
    calcStats(allTickets.value);
  } catch {
    notificationStore.error('Could not load tickets.');
  } finally {
    isLoading.value = false;
  }
};

const goToPage = async (p) => {
  if (p < 1 || p > totalPages.value) return;
  await fetchPage(p);
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const reload = async () => {
  await fetchPage(currentPage.value);
  notificationStore.success('Tickets updated.');
};

const openTicketDetail = (ticket) => {
  selectedTicket.value = ticket;
};

const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};

const createTicket = async () => {
  formErrors.value = { title: '', description: '' };
  let hasError = false;
  if (!newTicket.title.trim() || newTicket.title.trim().length < 5) {
    formErrors.value.title = 'Subject must be at least 5 characters.'; hasError = true;
  }
  if (!newTicket.description.trim() || newTicket.description.trim().length < 10) {
    formErrors.value.description = 'Description must be at least 10 characters.'; hasError = true;
  }
  if (hasError) return;
  creatingTicket.value = true;
  try {
    await axios.post(`${API_URL}/tickets`, {
      title:      newTicket.title.trim(),
      description: newTicket.description.trim(),
      priority:   parseInt(newTicket.priority)
    });
    showModal.value       = false;
    newTicket.title       = '';
    newTicket.description = '';
    notificationStore.success('Ticket submitted!');
    await fetchPage(1);
  } catch {
    notificationStore.error('Error submitting ticket.');
  } finally {
    creatingTicket.value = false;
  }
};

onMounted(async () => {
  if (!authStore.isAuthenticated) { router.push('/'); return; }
  await fetchPage(1);
  const closeMenu = (e) => {
    if (!e.target.closest('.profile-menu-container')) showProfileMenu.value = false;
  };
  const onKey = (e) => {
    if (['INPUT', 'TEXTAREA'].includes(e.target.tagName)) return;
    if (e.key === 'n' || e.key === 'N') showModal.value = true;
    if (e.key === 'r' || e.key === 'R') reload();
    if (e.key === 'Escape') { showModal.value = false; selectedTicket.value = null; }
    if (e.key === 'ArrowRight') goToPage(currentPage.value + 1);
    if (e.key === 'ArrowLeft')  goToPage(currentPage.value - 1);
  };
  window.addEventListener('click', closeMenu);
  window.addEventListener('keydown', onKey);
  onUnmounted(() => {
    window.removeEventListener('click', closeMenu);
    window.removeEventListener('keydown', onKey);
  });
});
</script>

<style scoped>
.page-fade-in { animation: fadeIn 0.4s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
.custom-scrollbar::-webkit-scrollbar { width: 5px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.1); border-radius: 10px; }
</style>
