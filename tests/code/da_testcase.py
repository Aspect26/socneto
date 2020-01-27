
def test_data_acquisition(kafka, 
    job_update_topic,
    posts_output_topic,
    jobId = "e3a7ce8b-2d10-46ff-ba91-925a5a99a3e9",
    attributes = {}):

    try:
        print("Create new job")
        kafka.produce_start_job(
                        job_update_topic,
                        outputMessageBrokerChannel = posts_output_topic,
                        jobId=jobId,
                        attributes = attributes)

        print("Waits for acquired posts")        
        for post in kafka.consume_topic(posts_output_topic):
            keys = [
                'originalPostId', 
                'postId', 
                'jobId', 
                'text', 
                'originalText', 
                'language', 
                'source', 
                'authorId','dateTime'
            ]
            print("Check that the message is in the correct format")            
            for k in keys:
                if k not in post:
                    raise Exception("Element {} not found in post {}".format(k,post))
            return (True,None)
            
    except Exception as e:
        return (False, "Testing data acquirer failed: {}".format(e))
    finally:
        print("Stop the job")
        kafka.produce_stop_job(job_update_topic,jobId)

