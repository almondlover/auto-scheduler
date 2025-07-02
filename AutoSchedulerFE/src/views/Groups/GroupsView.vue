<script setup lang="ts">
import type { Group } from '@/classes/group';
import GroupForm from '@/components/GroupForm.vue';
import GroupList from '@/components/GroupList.vue';
import Button from '@/components/ui/button/Button.vue';
import Dialog from '@/components/ui/dialog/Dialog.vue';
import DialogContent from '@/components/ui/dialog/DialogContent.vue';
import DialogTrigger from '@/components/ui/dialog/DialogTrigger.vue';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';

const groupStore = useGroupStore();
const { groups, currentOrganizationIdx } = storeToRefs(groupStore);

const handleSubmit = (newGroup:Group) => {
    newGroup.organizationId = currentOrganizationIdx.value;
    groupStore.createGroup({...newGroup});
};
</script>

<template>
  <main>
    <h1 class="page-title">Organization groups</h1>
    <Dialog>
      <DialogTrigger>
        <Button class="mx-10">New Group</Button>
      </DialogTrigger>
      <DialogContent>
        <GroupForm @added="e=>handleSubmit(e)"/>
      </DialogContent>
    </Dialog>
    <div class=" m-5 p-5 bg-secondary border-2 border-primary rounded-md ">
      <GroupList class="flex gap-10 flex-row flex-wrap"/>
    </div>
  </main>
</template>