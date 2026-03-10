<template>
  <div class="min-h-screen flex items-center justify-center bg-[#f3f2f1] dark:bg-gradient-to-br dark:from-[#1a1a1a] dark:to-[#090909] relative px-4 transition-colors">
    <div class="w-full max-w-[440px] bg-white dark:bg-[#242424] p-11 shadow-lg dark:shadow-[0_0_20px_rgba(0,0,0,0.5)] transition-colors">
      <div class="mb-6 flex items-center gap-2">
        <div class="w-6 h-6 bg-blue-600 rounded flex items-center justify-center">
            <TicketIcon class="w-4 h-4 text-white" />
        </div>
        <span class="font-semibold text-[15px] text-[#1b1b1b] dark:text-[#f3f2f1]">TicketSystem</span>
      </div>
      
      <h1 class="text-[24px] font-semibold text-[#1b1b1b] dark:text-white mb-1 tracking-tight">{{ t('auth.sign_in') }}</h1>
      <p class="text-[15px] text-[#1b1b1b] dark:text-[#f3f2f1] mb-6">{{ t('auth.continue_to') }}</p>
      
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
           <input 
              v-model="email" 
              type="email" 
              @input="formErrors.email = ''"
              class="w-full bg-transparent border-b border-[#8a8886] dark:border-[#8a8886] hover:border-[#1b1b1b] dark:hover:border-white text-[#1b1b1b] dark:text-white px-1 py-1.5 focus:border-[#0067b8] dark:focus:border-[#0067b8] focus:border-b-2 focus:ring-0 outline-none transition-all placeholder:text-[#605e5c] dark:placeholder:text-[#a19f9d]"
              :placeholder="t('auth.email_phone')"
            />
            <p v-if="formErrors.email" class="text-[#e81123] text-[13px] mt-1">{{ formErrors.email }}</p>
        </div>
        
        <div class="pt-1">
           <input 
              v-model="password" 
              type="password" 
              @input="formErrors.password = ''"
              class="w-full bg-transparent border-b border-[#8a8886] dark:border-[#8a8886] hover:border-[#1b1b1b] dark:hover:border-white text-[#1b1b1b] dark:text-white px-1 py-1.5 focus:border-[#0067b8] dark:focus:border-[#0067b8] focus:border-b-2 focus:ring-0 outline-none transition-all placeholder:text-[#605e5c] dark:placeholder:text-[#a19f9d]"
              :placeholder="t('auth.password')"
            />
            <p v-if="formErrors.password" class="text-[#e81123] text-[13px] mt-1">{{ formErrors.password }}</p>
        </div>

        <div class="pt-2">
            <div class="text-[13px] text-[#0067b8] dark:text-[#4da1ff] hover:text-[#005da6] dark:hover:text-[#6cb1ff] hover:underline cursor-pointer w-fit mb-2">
            <router-link to="/forgot-password">{{ t('auth.forgot') }}</router-link>
            </div>

            <div class="flex items-center gap-1.5">
            <span class="text-[13px] text-[#1b1b1b] dark:text-[#f3f2f1]">{{ t('auth.no_account') }}</span>
            <router-link to="/register" class="text-[13px] text-[#0067b8] dark:text-[#4da1ff] hover:text-[#005da6] dark:hover:text-[#6cb1ff] hover:underline focus:outline-none">
                {{ t('auth.create_one') }}
            </router-link>
            </div>
        </div>

        <div class="flex justify-end pt-4">
          <button 
            type="submit" 
            :disabled="authStore.loading"
            class="bg-[#0067b8] hover:bg-[#005da6] text-white px-8 py-1.5 min-w-[108px] font-semibold text-[15px] transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
          >
            <span v-if="!authStore.loading">{{ t('auth.next') }}</span>
            <Loader2Icon v-else class="w-5 h-5 animate-spin" />
          </button>
        </div>
        
        <ErrorMessage :message="authStore.error" />
      </form>

      <!-- Sign-in options bottom text -->
      <div class="mt-8">
        <div class="flex items-center gap-3 cursor-pointer group w-fit">
            <div class="w-8 h-8 rounded-full border border-[#8a8886] flex items-center justify-center">
                <TicketIcon class="w-4 h-4 text-[#8a8886] group-hover:text-[#1b1b1b] dark:group-hover:text-white transition-colors" />
            </div>
            <span class="text-[15px] text-[#1b1b1b] dark:text-[#f3f2f1] group-hover:text-black dark:group-hover:text-white transition-colors">{{ t('auth.login_options') }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { useNotificationStore } from '../store/notifications';
import { TicketIcon, Loader2Icon } from 'lucide-vue-next';
import ErrorMessage from '../components/ErrorMessage.vue';
import { useI18n } from '../composables/useI18n';

const { t } = useI18n();
const authStore = useAuthStore();
const notificationStore = useNotificationStore();
const router = useRouter();
const route = useRoute();

const email = ref('');
const password = ref('');
const formErrors = ref({ email: '', password: '' });

onMounted(() => {
  const params = new URLSearchParams(window.location.search);
  const msg = params.get('mensaje');
  if (msg) authStore.error = msg;

  if (route.query.expired === 'true') {
    notificationStore.error('Tu sesión ha expirado por seguridad. Por favor, iniciá sesión nuevamente.');
    router.replace('/');
  }
});

const handleLogin = async () => {
  formErrors.value = { email: '', password: '' };
  let hasError = false;

  if (!email.value) {
    formErrors.value.email = t('auth.email_req');
    hasError = true;
  }
  if (!password.value) {
    formErrors.value.password = t('auth.pass_req');
    hasError = true;
  }

  if (hasError) return;

  const success = await authStore.login(email.value, password.value);
  if (success) {
    notificationStore.success(`¡Bienvenido de nuevo, ${authStore.user?.nombre}!`);
    if (authStore.isAdmin || authStore.isOperador) {
      router.push('/admin');
    } else {
      router.push('/dashboard');
    }
  }
};
</script>
