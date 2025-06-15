<script setup lang="ts">
import { ref, watch } from 'vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import Button from './ui/button/Button.vue';
import { useGroupStore } from '@/stores/groupStore';
import MemberForm from './MemberForm.vue';

const showNewMemberModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx, members} = storeToRefs(groupStore);

watch(currentOrganizationIdx, ()=>{
    members.value = groupStore.organization(currentOrganizationIdx.value).value?.members??[];
});
</script>

<template>
    <div v-for="member in members" class="flex h-15 items-center space-x-5">
        <div>
            {{member.name}}
        </div>
        <Separator orientation="vertical"/>
        <div>
            {{member.contact}}
        </div>
        <Button @click="groupStore.removeMember(member.id)">Delete</Button>
    </div>
    <Button @click="showNewMemberModal=!showNewMemberModal">New Member</Button>
    <MemberForm v-show="showNewMemberModal"/>
</template>